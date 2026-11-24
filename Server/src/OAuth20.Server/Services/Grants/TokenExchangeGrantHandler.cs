using OAuth20.Server.Common;
using OAuth20.Server.Enumeration;
using OAuth20.Server.Models;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Grants
{
    /// <summary>
    /// RFC 8693 token exchange (simplified): exchanges an access token issued by this server
    /// for a new, possibly down-scoped, access token for another audience.
    /// </summary>
    public class TokenExchangeGrantHandler : GrantHandlerBase
    {
        public const string AccessTokenType = "urn:ietf:params:oauth:token-type:access_token";

        public TokenExchangeGrantHandler(IClientService clientService, ITokenIssuer tokenIssuer)
            : base(clientService, tokenIssuer) { }

        public override string GrantType => AuthorizationGrantTypesEnum.TokenExchange.GetEnumDescription();

        protected override bool RequiresClientSecret => true;

        protected override Task<TokenResponse> HandleAsync(TokenRequest request, Client client)
        {
            var invalidRequest = ErrorTypeEnum.InvalidRequest.GetEnumDescription();

            var subjectToken = TokenIssuer.ValidateAccessToken(request.subject_token);
            if (subjectToken == null)
                return Done(Error(invalidRequest, "subject_token is invalid, expired or revoked"));

            // scopes: what the subject token has, limited to what this client may use,
            // and (if requested) limited again to the requested ones.
            var subjectScopes = subjectToken.Claims.FirstOrDefault(x => x.Type == "scope")?.Value
                ?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var scopes = subjectScopes.Intersect(client.AllowedScopes).ToList();

            if (request.scope != null && request.scope.Any())
            {
                if (request.scope.Except(scopes).Any())
                    return Done(Error(ErrorTypeEnum.InValidScope.GetEnumDescription(), "requested scope is not allowed for this exchange"));
                scopes = request.scope.ToList();
            }

            if (!scopes.Any())
                return Done(Error(ErrorTypeEnum.InValidScope.GetEnumDescription(), "no usable scope"));

            // audience: must be a registered client (by id or uri)
            string audience = null;
            if (!string.IsNullOrWhiteSpace(request.audience))
            {
                var target = ClientStore.Instance.Clients.FirstOrDefault(x =>
                    x.IsActive && (x.ClientId == request.audience || x.ClientUri == request.audience));
                if (target == null)
                    return Done(Error("invalid_target", "unknown audience"));
                audience = target.ClientUri ?? target.ClientId;
            }

            // actor: who is acting on behalf of the subject (the actor token's subject, or this client)
            var actor = client.ClientId;
            if (!string.IsNullOrEmpty(request.actor_token))
            {
                var actorToken = TokenIssuer.ValidateAccessToken(request.actor_token);
                if (actorToken == null)
                    return Done(Error(invalidRequest, "actor_token is invalid, expired or revoked"));
                actor = actorToken.Subject ?? client.ClientId;
            }

            var extraClaims = new List<Claim>
            {
                new Claim("act", JsonSerializer.Serialize(new Dictionary<string, string> { ["sub"] = actor }), JsonClaimValueTypes.Json),
                new Claim("client_id", client.ClientId)
            };

            var token = TokenIssuer.CreateAccessToken(scopes, client, subjectToken.Subject, audience, extraClaims);

            return Done(new TokenResponse
            {
                access_token = token.AccessToken,
                issued_token_type = AccessTokenType
            });
        }

        private static Task<TokenResponse> Done(TokenResponse response) => Task.FromResult(response);
    }
}
