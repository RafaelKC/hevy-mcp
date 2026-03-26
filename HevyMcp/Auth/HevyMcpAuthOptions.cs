namespace HevyMcp.Auth;

public sealed class HevyMcpAuthOptions
{
    /// <summary>
    /// 32-byte master key, base64 encoded. Used to encrypt/decrypt bearer tokens (AES-GCM).
    /// </summary>
    public string MasterKeyBase64 { get; set; } = "";

    /// <summary>
    /// Default token lifetime in minutes.
    /// </summary>
    public int TokenTtlMinutes { get; set; } = 60;
}

