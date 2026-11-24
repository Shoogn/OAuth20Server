namespace OAuth20.Server.Infrastructure;

public interface IOAuth2JwtEncoder
{
    string Encode(OAuth2JwtHeader header, OAuth2JwtPayload payload);
}
