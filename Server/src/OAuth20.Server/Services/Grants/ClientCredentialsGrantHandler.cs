using OAuth20.Server.Common;
using OAuth20.Server.Models;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Grants
{
    public class ClientCredentialsGrantHandler : GrantHandlerBase
    {
        public ClientCredentialsGrantHandler(IClientService clientService, ITokenIssuer tokenIssuer)
            : base(clientService, tokenIssuer) { }

        public override string GrantType => AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription();

        protected override bool RequiresClientSecret => true;

        protected override Task<TokenResponse> HandleAsync(TokenRequest request, Client client)
        {
            var scopes = request.scope == null
                ? client.AllowedScopes
                : client.AllowedScopes.Intersect(request.scope).ToList();

            if (!scopes.Any())
                return Task.FromResult(Error("invalid_scope", "none of the requested scopes are allowed for this client"));

            var token = TokenIssuer.CreateAccessToken(scopes, client, null);
            return Task.FromResult(new TokenResponse { access_token = token.AccessToken, id_token = null, code = request.code });
        }
    }
}
