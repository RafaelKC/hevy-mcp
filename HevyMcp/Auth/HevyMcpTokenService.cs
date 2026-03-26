using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HevyMcp.Auth;

public sealed class HevyMcpTokenService
{
    public const string TokenPrefix = "hevy-mcp.v1.";

    private readonly byte[] _masterKey;

    public HevyMcpTokenService(HevyMcpAuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.MasterKeyBase64))
            throw new InvalidOperationException(
                "Missing HevyMcp auth master key. Set HEVY_MCP_MASTER_KEY_BASE64 (32 bytes, base64-encoded).");

        _masterKey = Convert.FromBase64String(options.MasterKeyBase64);
        if (_masterKey.Length != 32)
            throw new InvalidOperationException("HevyMcp master key must be 32 bytes (base64-encoded).");
    }

    public sealed record TokenPayload(
        string HevyApiKey,
        string? HevyBaseUrl,
        long IatUnixSeconds,
        long ExpUnixSeconds);

    public string IssueToken(string hevyApiKey, string? hevyBaseUrl, DateTimeOffset now, TimeSpan ttl)
    {
        var iat = now.ToUnixTimeSeconds();
        var exp = now.Add(ttl).ToUnixTimeSeconds();

        var payload = new TokenPayload(
            HevyApiKey: hevyApiKey,
            HevyBaseUrl: string.IsNullOrWhiteSpace(hevyBaseUrl) ? null : hevyBaseUrl,
            IatUnixSeconds: iat,
            ExpUnixSeconds: exp);

        var plaintext = JsonSerializer.SerializeToUtf8Bytes(payload);

        Span<byte> nonce = stackalloc byte[12];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        Span<byte> tag = stackalloc byte[16];

        using var aes = new AesGcm(_masterKey, tagSizeInBytes: 16);
        aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData: Encoding.UTF8.GetBytes(TokenPrefix));

        // token bytes = nonce || ciphertext || tag
        var tokenBytes = new byte[nonce.Length + ciphertext.Length + tag.Length];
        nonce.CopyTo(tokenBytes.AsSpan(0, nonce.Length));
        ciphertext.CopyTo(tokenBytes.AsSpan(nonce.Length, ciphertext.Length));
        tag.CopyTo(tokenBytes.AsSpan(nonce.Length + ciphertext.Length, tag.Length));

        return TokenPrefix + Base64Url.Encode(tokenBytes);
    }

    public TokenPayload ValidateAndDecode(string token, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(token) || !token.StartsWith(TokenPrefix, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Invalid bearer token prefix.");

        var blob = token.Substring(TokenPrefix.Length);
        var tokenBytes = Base64Url.Decode(blob);

        if (tokenBytes.Length < 12 + 16)
            throw new UnauthorizedAccessException("Invalid bearer token length.");

        var nonce = tokenBytes.AsSpan(0, 12).ToArray();
        var tag = tokenBytes.AsSpan(tokenBytes.Length - 16, 16).ToArray();
        var ciphertext = tokenBytes.AsSpan(12, tokenBytes.Length - 12 - 16).ToArray();

        var plaintext = new byte[ciphertext.Length];

        try
        {
            using var aes = new AesGcm(_masterKey, tagSizeInBytes: 16);
            aes.Decrypt(nonce, ciphertext, tag, plaintext, associatedData: Encoding.UTF8.GetBytes(TokenPrefix));
        }
        catch (CryptographicException)
        {
            throw new UnauthorizedAccessException("Invalid bearer token (failed to decrypt).");
        }

        var payload = JsonSerializer.Deserialize<TokenPayload>(plaintext);
        if (payload is null || string.IsNullOrWhiteSpace(payload.HevyApiKey))
            throw new UnauthorizedAccessException("Invalid bearer token payload.");

        if (payload.ExpUnixSeconds <= now.ToUnixTimeSeconds())
            throw new UnauthorizedAccessException("Bearer token expired.");

        return payload;
    }
}

