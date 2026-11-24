using OAuth20.Server.Models;
using OAuth20.Server.OauthResponse;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OAuth20.Server.Services.Grants
{
    /// <summary>
    /// Creates and stores signed JWT tokens, shared by all grant handlers.
    /// </summary>
    public interface ITokenIssuer
    {
        TokenResult CreateAccessToken(IEnumerable<string> scopes, Client client, string sub,
            string audience = null, IEnumerable<Claim> extraClaims = null);

        string CreateIdToken(Client client, string userId, string userName, string nonce);

        /// <summary>Validates our own access token (signature, issuer, lifetime, not revoked).</summary>
        JwtSecurityToken ValidateAccessToken(string token);
    }
}
