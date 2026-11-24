using OAuth20.Server.Common;
using OAuth20.Server.Enumeration;
using OAuth20.Server.Models;
using OAuth20.Server.Models.Context;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Grants
{
    public class DeviceCodeGrantHandler : GrantHandlerBase
    {
        private readonly BaseDBContext _context;

        public DeviceCodeGrantHandler(IClientService clientService, ITokenIssuer tokenIssuer, BaseDBContext context)
            : base(clientService, tokenIssuer)
        {
            _context = context;
        }

        public override string GrantType => AuthorizationGrantTypesEnum.DeviceCode.GetEnumDescription();

        protected override Task<TokenResponse> HandleAsync(TokenRequest request, Client client)
        {
            var deviceCode = _context.DeviceFlows
                .SingleOrDefault(x => x.DeviceCode == request.device_code && x.ClientId == request.client_id);

            if (deviceCode == null)
                return Task.FromResult(Error(ErrorTypeEnum.InvalidRequest.GetEnumDescription(), "Please check the device code"));

            if (deviceCode.UserInterActionComplete != true)
                return Task.FromResult(Error(ErrorTypeEnum.WaitForUserInteraction.GetEnumDescription()));

            if (deviceCode.ExpireIn < DateTime.Now)
                return Task.FromResult(Error(ErrorTypeEnum.InvalidRequest.GetEnumDescription(), "The device code is expired"));

            var scopes = client.AllowedScopes.Intersect(deviceCode.RequestedScope.Split(' '));
            var token = TokenIssuer.CreateAccessToken(scopes, client, null);

            return Task.FromResult(new TokenResponse { access_token = token.AccessToken, id_token = null });
        }
    }
}
