namespace OAuth20.Server.Infrastructure;

public interface IOAuth2JwtDecoder
{
    OAuth2DecodedJwt Decode(string token);
}
