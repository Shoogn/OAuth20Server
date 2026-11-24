using System;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OAuth20.Server.Infrastructure;

public class OAuth2Base64Url
{
    public static string Encode(Span<byte> data)
    {
        return Convert.ToBase64String(data)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static byte[] Decode(string base64Url)
    {
        string base64 = base64Url
            .Replace('-', '+')
            .Replace('_', '/');


        // Pad with '=' characters to make the length a multiple of 4
        // Base64 strings must be a multiple of 4 characters long
        // if the length is not a multiple of 4, we need to add padding
        // The padding character '=' is used to fill in the missing characters
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}

public static class JwtValidator
{
    public static OAuth2JwtValidationResult Validate(string token, RSA publicKey, string expectedIssuer, string expectedAudience, TimeSpan? clockSkew = null)
    {
        clockSkew ??= TimeSpan.FromMinutes(5);

        try
        {
            string[] parts = token.Split('.');

            if (parts.Length != 3)
            {
                return new()
                {
                    IsValid = false,
                    Error = "Invalid JWT format"
                };
            }

            string unsigned =
                $"{parts[0]}.{parts[1]}";

            byte[] signature =
                OAuth2Base64Url.Decode(parts[2]);

            bool signatureValid =
                publicKey.VerifyData(
                    Encoding.UTF8.GetBytes(unsigned),
                    signature,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

            if (!signatureValid)
            {
                return new()
                {
                    IsValid = false,
                    Error = "Invalid signature"
                };
            }

            string payloadJson =
                Encoding.UTF8.GetString(
                    OAuth2Base64Url.Decode(parts[1]));

            JsonDocument payload =
                JsonDocument.Parse(payloadJson);

            var root = payload.RootElement;

            long now =
                DateTimeOffset.UtcNow
                    .ToUnixTimeSeconds();

            if (!root.TryGetProperty("exp", out var exp))
            {
                return new()
                {
                    IsValid = false,
                    Error = "Missing exp"
                };
            }

            if (exp.GetInt64() < now - clockSkew.Value.TotalSeconds)
            {
                return new()
                {
                    IsValid = false,
                    Error = "Token expired"
                };
            }

            if (root.TryGetProperty("nbf", out var nbf))
            {
                if (nbf.GetInt64() >
                    now + clockSkew.Value.TotalSeconds)
                {
                    return new()
                    {
                        IsValid = false,
                        Error = "Token not active yet"
                    };
                }
            }

            if (!root.TryGetProperty("iss", out var iss))
            {
                return new()
                {
                    IsValid = false,
                    Error = "Missing issuer"
                };
            }

            if (iss.GetString() != expectedIssuer)
            {
                return new()
                {
                    IsValid = false,
                    Error = "Invalid issuer"
                };
            }

            if (!root.TryGetProperty("aud", out var aud))
            {
                return new()
                {
                    IsValid = false,
                    Error = "Missing audience"
                };
            }

            if (aud.GetString() != expectedAudience)
            {
                return new()
                {
                    IsValid = false,
                    Error = "Invalid audience"
                };
            }

            return new()
            {
                IsValid = true,
                Payload = payload
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Error = ex.Message
            };
        }
    }
}