using System.Text.Json;

namespace OAuth20.Server.Infrastructure;

public sealed class OAuth2JwtValidationResult
{
    public bool IsValid { get; init; }
    public string Error { get; init; } = string.Empty;

    public JsonDocument Payload { get; init; }
}
