using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Configuration;
using OAuth20.Server.Models;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Models.Entities;
using OAuth20.Server.OauthResponse;
using OAuth20.Server.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace OAuth20.Server.Services.Grants
{
    public class TokenIssuer : ITokenIssuer
    {
        private const int LifetimeMinutes = 50;

        private readonly OAuthServerOptions _options;
        private readonly ISigningKeyStore _signingKeyStore;
        private readonly BaseDBContext _context;

        public TokenIssuer(IOptions<OAuthServerOptions> options, ISigningKeyStore signingKeyStore, BaseDBContext context)
        {
            _options = options.Value;
            _signingKeyStore = signingKeyStore;
            _context = context;
        }

        public TokenResult CreateAccessToken(IEnumerable<string> scopes, Client client, string sub,
            string audience = null, IEnumerable<Claim> extraClaims = null)
        {
            var claims = new List<Claim> { new Claim("scope", string.Join(' ', scopes)) };

            if (!string.IsNullOrEmpty(sub))
                claims.Add(new Claim("sub", sub, ClaimValueTypes.String));

            if (extraClaims != null)
                claims.AddRange(extraClaims);

            var token = new JwtSecurityToken(_options.IDPUri, audience ?? client.ClientUri, claims,
                notBefore: DateTime.UtcNow, expires: DateTime.UtcNow.AddMinutes(LifetimeMinutes),
                signingCredentials: new SigningCredentials(new RsaSecurityKey(_signingKeyStore.GetPrivateKey()), SecurityAlgorithms.RsaSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            Save(client.ClientId, accessToken, Constants.TokenTypes.JWTAcceseccToken, token.ValidTo, sub, Constants.TokenTypeHints.AccessToken);

            return new TokenResult
            {
                AccessToken = accessToken,
                TokenType = Constants.TokenTypes.JWTAcceseccToken,
                ExpirationDate = token.ValidTo
            };
        }

        public string CreateIdToken(Client client, string userId, string userName, string nonce)
        {
            var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var claims = new List<Claim>
            {
                new Claim("sub", userId, ClaimValueTypes.String),
                new Claim("given_name", userName, ClaimValueTypes.String),
                new Claim("iat", iat.ToString(), ClaimValueTypes.Integer64),
                new Claim("nonce", nonce ?? string.Empty, ClaimValueTypes.String),
                new Claim("amr", "pwd")
            };

            var token = new JwtSecurityToken(_options.IDPUri, client.ClientId, claims,
                expires: DateTime.UtcNow.AddMinutes(LifetimeMinutes),
                signingCredentials: new SigningCredentials(new RsaSecurityKey(_signingKeyStore.GetPrivateKey()), SecurityAlgorithms.RsaSha256));

            var idToken = new JwtSecurityTokenHandler().WriteToken(token);
            Save(client.ClientId, idToken, Constants.TokenTypes.JWTIdentityToken, token.ValidTo, userId, null);
            return idToken;
        }

        public JwtSecurityToken ValidateAccessToken(string token)
        {
            var parameters = new TokenValidationParameters
            {
                IssuerSigningKey = new RsaSecurityKey(_signingKeyStore.GetPublicKey(Constants.OAuth2JwtConstants.CurrentKeyId)),
                ValidIssuer = _options.IDPUri,
                ValidateIssuer = true,
                ValidateAudience = false, // any client of this server may be the audience
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, parameters, out var validated);

                var stored = _context.OAuthTokens.FirstOrDefault(x => x.Token == token);
                if (stored == null || stored.Revoked)
                    return null;

                return validated as JwtSecurityToken;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Save(string clientId, string token, string tokenType, DateTime expires, string subjectId, string hint)
        {
            _context.OAuthTokens.Add(new OAuthTokenEntity
            {
                ClientId = clientId,
                CreationDate = DateTime.Now,
                ReferenceId = Guid.NewGuid().ToString(),
                Status = Constants.Statuses.Valid,
                Token = token,
                TokenType = tokenType,
                TokenTypeHint = hint,
                ExpirationDate = expires,
                SubjectId = subjectId,
                Revoked = false
            });
            _context.SaveChanges();
        }
    }
}
