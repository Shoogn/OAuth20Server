using OAuth20.Server.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OAuth20.Server.Infrastructure;

public sealed class OAuth2JwtEncoder : IOAuth2JwtEncoder
{
    private readonly ISigningKeyStore _keys;
    public OAuth2JwtEncoder(ISigningKeyStore signingKeyStore)
    {
        _keys = signingKeyStore;
    }
    public string Encode(OAuth2JwtHeader header, OAuth2JwtPayload payload)
    {
        string headerJson = JsonSerializer.Serialize(header);
        string payloadJson = JsonSerializer.Serialize(payload);
        string headerPart = OAuth2Base64Url.Encode(Encoding.UTF8.GetBytes(headerJson));
        string payloadPart = OAuth2Base64Url.Encode(Encoding.UTF8.GetBytes(payloadJson));

        string unsigned = $"{headerPart}.{payloadPart}";
        byte[] signature = _keys.GetPrivateKey().SignData(Encoding.UTF8.GetBytes(unsigned), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        string signaturePart = OAuth2Base64Url.Encode(signature);

        return $"{unsigned}.{signaturePart}";
        //string headerJson =
        //    JsonSerializer.Serialize(header);
        //var mergedPayload =
        //    new Dictionary<string, object>
        //    {
        //        ["sub"] = payload.Sub,
        //        ["iss"] = payload.Iss,
        //        ["aud"] = payload.Aud,
        //        ["iat"] = payload.Iat,
        //        ["exp"] = payload.Exp,
        //        ["nbf"] = payload.Nbf,
        //        ["jti"] = payload.Jti,
        //        ["typ"] = payload.TokenType
        //    };
        //foreach (var kv in payload.Claims)
        //    mergedPayload[kv.Key] = kv.Value;
        //string payloadJson =
        //    JsonSerializer.Serialize(mergedPayload);
        //string headerPart =
        //    OAuth2Base64Url.Encode(
        //        Encoding.UTF8.GetBytes(headerJson));
        //string payloadPart =
        //    OAuth2Base64Url.Encode(
        //        Encoding.UTF8.GetBytes(payloadJson));
        //string unsignedToken =
        //    $"{headerPart}.{payloadPart}";
        //byte[] signature =
        //    _privateKey.SignData(
        //        Encoding.UTF8.GetBytes(unsignedToken),
        //        HashAlgorithmName.SHA256,
        //        RSASignaturePadding.Pkcs1);
        //string signaturePart =
        //    OAuth2Base64Url.Encode(signature);
        //return $"{unsignedToken}.{signaturePart}";
    }
}
