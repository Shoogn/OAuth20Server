using static OAuth20.Server.Constants;

namespace OAuth20.Server.Infrastructure;

public sealed class OAuth2JwtHeader
{
    public string Alg { get; init; } = "RS256";
    public string Typ { get; init; } = "JWT";
    public string Kid { get; init; } = OAuth2JwtConstants.CurrentKeyId;
}
