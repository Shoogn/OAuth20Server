using OAuth20.Server.Models.Context;
using OAuth20.Server.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Text;
using OAuth20.Server.OauthResponse;
using System.Linq;
using OAuth20.Server.Common;

namespace OAuth20.Server.Validations
{
    public class TokenRevocationValidation : ITokenRevocationValidation
    {
        private readonly BaseDBContext _dBContext;
        private readonly ClientStore _clientStore = new ClientStore();
        public TokenRevocationValidation(BaseDBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public virtual async Task<TokenRevocationValidationResponse> ValidateAsync(HttpContext context)
        {
            var response = new TokenRevocationValidationResponse { Succeeded = true };
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (authorizationHeader == null)
            {
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return response;
            }
            if (!authorizationHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
            {
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return response;
            }
            try
            {
                var parameters = authorizationHeader.Substring("Basic ".Length);
                var authorizationKeys = Encoding.UTF8.GetString(Convert.FromBase64String(parameters));

                var authorizationResult = authorizationKeys.IndexOf(':');
                if(authorizationResult == -1)
                {
                    response.Succeeded = false;
                    response.Error = "Client is not Authorized";
                    return response;
                }
                string clinetId = authorizationKeys.Substring(0, authorizationResult);
                string clientSecret = authorizationKeys.Substring(authorizationResult + 1);

                // Here I have to get the client from the Client Store
                var client = VerifyClientById(clinetId, true, clientSecret);
                if (!client.IsSuccess)
                {
                    response.Succeeded = false;
                    response.Error = "Client is not Authorized";
                    return response;
                }

                response.ClientId = clinetId;
            }
            catch (Exception ex)
            {
                _ = ex;
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return response;
            }
             return response;
        }



        private CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = null)
        {
            CheckClientResult result = new CheckClientResult() { IsSuccess = false };

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = _clientStore.Clients.Where(x => x.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (client != null)
                {
                    if (checkWithSecret && !string.IsNullOrEmpty(clientSecret))
                    {
                        bool hasSamesecretId = client.ClientSecret.Equals(clientSecret, StringComparison.InvariantCulture);
                        if (!hasSamesecretId)
                        {
                            result.Error = ErrorTypeEnum.InvalidClient.GetEnumDescription();
                            return result;
                        }
                    }
                    // check if client is enabled or not

                    if (client.IsActive)
                    {
                        result.IsSuccess = true;
                        result.Client = client;

                        return result;
                    }
                    else
                    {
                        result.ErrorDescription = ErrorTypeEnum.UnAuthoriazedClient.GetEnumDescription();
                        return result;
                    }
                }
            }

            result.ErrorDescription = ErrorTypeEnum.AccessDenied.GetEnumDescription();
            return result;
        }
    }
}
