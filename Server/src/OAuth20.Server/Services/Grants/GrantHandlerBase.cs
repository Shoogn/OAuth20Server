using OAuth20.Server.Common;
using OAuth20.Server.Enumeration;
using OAuth20.Server.Models;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Grants
{
    public abstract class GrantHandlerBase : IGrantHandler
    {
        protected readonly IClientService ClientService;
        protected readonly ITokenIssuer TokenIssuer;

        protected GrantHandlerBase(IClientService clientService, ITokenIssuer tokenIssuer)
        {
            ClientService = clientService;
            TokenIssuer = tokenIssuer;
        }

        public abstract string GrantType { get; }

        /// <summary>Whether the client must authenticate with its secret for this grant.</summary>
        protected virtual bool RequiresClientSecret => false;

        public async Task<TokenResponse> HandleAsync(TokenRequest request)
        {
            var check = ClientService.VerifyClientById(request.client_id, RequiresClientSecret, request.client_secret, GrantType);
            if (!check.IsSuccess)
                return Error(check.Error ?? ErrorTypeEnum.InvalidClient.GetEnumDescription(), check.ErrorDescription);

            if (!ClientAllowsGrant(check.Client))
                return Error(ErrorTypeEnum.UnAuthoriazedClient.GetEnumDescription(), "client is not allowed to use this grant type");

            return await HandleAsync(request, check.Client);
        }

        protected virtual bool ClientAllowsGrant(Client client) => client.GrantTypes?.Contains(GrantType) == true;

        protected abstract Task<TokenResponse> HandleAsync(TokenRequest request, Client client);

        protected static TokenResponse Error(string error, string description = null) =>
            new TokenResponse { Error = error, ErrorDescription = description };
    }
}
