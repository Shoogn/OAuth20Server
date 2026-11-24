using System.Text.Json;

namespace OAuth20.Server.Infrastructure;

public sealed class OAuth2DecodedJwt
{
    public OAuth2JwtHeader Header { get; init; }

    public JsonDocument Payload { get; init; }

    public byte[] Signature { get; init; }

    public string UnsignedToken { get; init; }
}
