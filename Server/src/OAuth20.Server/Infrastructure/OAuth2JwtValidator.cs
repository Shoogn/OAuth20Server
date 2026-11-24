using OAuth20.Server.Configuration;
using OAuth20.Server.Services.Interfaces;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth20.Server.Infrastructure;

public sealed class OAuth2JwtValidator : IOAuth2JwtValidator
{
    private readonly IOAuth2JwtDecoder _decoder;
    private readonly ISigningKeyStore _keys;
    private readonly ITokenRevocationStore _revocationStore;

    public OAuth2JwtValidator(
        IOAuth2JwtDecoder decoder,
        ISigningKeyStore keys,
        ITokenRevocationStore revocationStore
        )
    {
        _decoder = decoder;
        _keys = keys;
        _revocationStore = revocationStore;
    }

    public async Task<OAuth2JwtValidationResult> ValidateAsync(string token, OAuthServerOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var decoded = _decoder.Decode(token);
            var header = decoded.Header;

            //
            // Header validation
            //

            if (!string.Equals(header.Alg, "RS256", StringComparison.Ordinal))
            {
                return Invalid("Unsupported algorithm");
            }

            if (!string.Equals(header.Typ, "JWT", StringComparison.OrdinalIgnoreCase))
            {
                return Invalid("Invalid token type");
            }

            //
            // Signature validation
            //

            bool signatureValid = _keys.GetPublicKey(header.Kid).VerifyData(System.Text.Encoding.UTF8.GetBytes(decoded.UnsignedToken),
                        decoded.Signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            if (!signatureValid)
                return Invalid("Invalid signature");

            var payload = decoded.Payload.RootElement;

            //
            // Issuer
            //

            if (!payload.TryGetProperty("iss", out var iss))
            {
                return Invalid("Missing issuer");
            }

            if (!string.Equals(iss.GetString(), options.Issuer, StringComparison.Ordinal))
            {
                return Invalid("Invalid issuer");
            }

            //
            // Audience
            //

            if (!payload.TryGetProperty("aud", out var aud))
            {
                return Invalid("Missing audience");
            }

            if (!string.Equals(aud.GetString(), options.Audience, StringComparison.Ordinal))
            {
                return Invalid("Invalid audience");
            }

            //
            // Lifetime
            //

            if (options.ValidateLifetime)
            {
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (!payload.TryGetProperty("exp", out var exp))
                {
                    return Invalid("Missing exp");
                }

                if (exp.GetInt64() < now - (long)options.ClockSkew.Value.TotalSeconds)
                {
                    return Invalid("Token expired");
                }

                if (payload.TryGetProperty("nbf", out var nbf))
                {
                    if (nbf.GetInt64() > now + (long)options.ClockSkew.Value.TotalSeconds)
                    {
                        return Invalid("Token not active");
                    }
                }
            }

            //
            // Revocation
            //

            if (payload.TryGetProperty(
                "jti",
                out var jti))
            {
                bool revoked = await _revocationStore.IsRevokedAsync(jti.GetString()!, cancellationToken);
                if (revoked)
                    return Invalid("Token revoked");
            }

            return new OAuth2JwtValidationResult
            {
                IsValid = true,
                Payload = decoded.Payload
            };
        }
        catch (Exception ex)
        {
            return Invalid(ex.Message);
        }
    }


    private static OAuth2JwtValidationResult Invalid(string error)
    {
        return new OAuth2JwtValidationResult
        {
            IsValid = false,
            Error = error
        };
    }

}
