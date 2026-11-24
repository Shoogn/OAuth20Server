using System;
using System.Text;
using System.Text.Json;

namespace OAuth20.Server.Infrastructure;

// This will replace:
// JwtSecurityTokenHandler.ReadJwtToken(...)
// does not validate the token, it only parses it.
// So we can use it in the decoder to parse the token without validating it, and then we can use the JwtSecurityTokenHandler.ValidateToken(...)
// to validate the token in the validation step. This way we can separate the concerns of decoding and validating the token.
public sealed class OAuth2JwtDecoder : IOAuth2JwtDecoder
{
    public OAuth2DecodedJwt Decode(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException(nameof(token));

        string[] parts = token.Split('.');

        if (parts.Length != 3)
            throw new InvalidOperationException("Invalid JWT format.");

        string headerJson = Encoding.UTF8.GetString(OAuth2Base64Url.Decode(parts[0]));
        string payloadJson = Encoding.UTF8.GetString(OAuth2Base64Url.Decode(parts[1]));
        OAuth2JwtHeader header = JsonSerializer.Deserialize<OAuth2JwtHeader>(headerJson);

        if (header == null)
            throw new InvalidOperationException("Unable to parse JWT header.");

        return new OAuth2DecodedJwt
        {
            Header = header,
            Payload = JsonDocument.Parse(payloadJson),
            Signature = OAuth2Base64Url.Decode(parts[2]),
            UnsignedToken = $"{parts[0]}.{parts[1]}"
        };
    }

}
