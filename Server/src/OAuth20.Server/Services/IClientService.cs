/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using OAuth20.Server.Common;
using OAuth20.Server.Models;
using System.Linq;
using System.Text;
using System;
using Microsoft.AspNetCore.Http;
using OAuth20.Server.Enumeration;

namespace OAuth20.Server.Services
{
    public interface IClientService
    {
        CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false,string clientSecret = null,
            string grantType = null);

        bool SearchForClientBySecret(string grantType);
    }

    public class ClientService : IClientService
    {
        private readonly ClientStore _clientStore = new ClientStore();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClientService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CheckClientResult VerifyClientById(string clientId,  bool checkWithSecret = false, string clientSecret = null, 
            string grantType = null)
        {
            CheckClientResult result = new CheckClientResult() { IsSuccess = false };

            if (!string.IsNullOrWhiteSpace(grantType) && 
                grantType == AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription())
            {
                var data = _httpContextAccessor.HttpContext;
                var authHeader = data.Request.Headers["Authorization"].ToString();
                if (authHeader == null)
                    return result;
                if (!authHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
                    return result;

                var parameters = authHeader.Substring("Basic ".Length);
                var authorizationKeys = Encoding.UTF8.GetString(Convert.FromBase64String(parameters));

                var authorizationResult = authorizationKeys.IndexOf(':');
                if (authorizationResult == -1)
                    return result;
                clientId = authorizationKeys.Substring(0, authorizationResult);
                clientSecret = authorizationKeys.Substring(authorizationResult + 1);

            }



            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = _clientStore
                    .Clients
                    .Where(x => 
                    x.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

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


        public bool SearchForClientBySecret(string grantType)
        {
            if (grantType == AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription() ||
                grantType == AuthorizationGrantTypesEnum.RefreshToken.GetEnumDescription() ||
                grantType == AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription())
                return true;

            return false;
        }
    }
}
