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
using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.AspNetCore.Http;
using OAuth20.Server.Enumeration;
using OAuth20.Server.Models.Context;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OAuth20.Server.Services
{
    public class ClientService : IClientService
    {
        private readonly ClientStore _clientStore = new ClientStore();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BaseDBContext _dbContext;
        public ClientService(IHttpContextAccessor httpContextAccessor,
            BaseDBContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = context;
        }

        public Task<CheckClientResult> GetClientByIdAsync(string clientId)
        {
            var c = _clientStore.Clients.Where(x => x.ClientId == clientId).FirstOrDefault();
            var response = new CheckClientResult
            {
                Client = c,
                IsSuccess = true
            };
            return Task.FromResult(response);
            //var client = await _dbContext.OAuthApplications.FirstOrDefaultAsync
            //    (x=>x.ClientId ==  clientId);
            //if(client == null)
            //{

            //}
            //else
            //{
            //   var c = _clientStore.Clients.Where(x => x.ClientId == clientId).FirstOrDefault();
            //}
        }


        public Task<CheckClientResult> GetClientByUriAsync(string clientUrl)
        {
            var c = _clientStore.Clients.Where(x => x.ClientUri == clientUrl).FirstOrDefault();
            var response = new CheckClientResult
            {
                Client = c,
                IsSuccess = true
            };
            return Task.FromResult(response);
        }

        public CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = null,
            string grantType = null)
        {
            CheckClientResult result = new CheckClientResult() { IsSuccess = false };

            if (!string.IsNullOrWhiteSpace(grantType) &&
                grantType == AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription())
            {
                result.Error = ErrorTypeEnum.InvalidClient.GetEnumDescription();

                var data = _httpContextAccessor.HttpContext;
                var authHeader = data.Request.Headers["Authorization"].ToString();
                if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                    return result;

                string authorizationKeys;
                try
                {
                    authorizationKeys = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Substring("Basic ".Length).Trim()));
                }
                catch (FormatException)
                {
                    return result;
                }

                var authorizationResult = authorizationKeys.IndexOf(':');
                if (authorizationResult == -1)
                    return result;

                result.Error = null;
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
                    if (checkWithSecret)
                    {
                        // a secret is mandatory when the caller asks for it
                        bool hasSameSecret = !string.IsNullOrEmpty(clientSecret) && CryptographicOperations.FixedTimeEquals(
                            Encoding.UTF8.GetBytes(client.ClientSecret ?? string.Empty), Encoding.UTF8.GetBytes(clientSecret));
                        if (!hasSameSecret)
                        {
                            result.Error = ErrorTypeEnum.InvalidClient.GetEnumDescription();
                            result.ErrorDescription = "client authentication failed";
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
                        result.Error = ErrorTypeEnum.UnAuthoriazedClient.GetEnumDescription();
                        result.ErrorDescription = "client is not active";
                        return result;
                    }
                }
            }

            result.Error = ErrorTypeEnum.InvalidClient.GetEnumDescription();
            result.ErrorDescription = "unknown client";
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

        public AudienceValidator ValidateAudienceHandler(IEnumerable<string> audiences, SecurityToken securityToken,
            TokenValidationParameters validationParameters, Client client, string token)
        {
            Func<IEnumerable<string>, SecurityToken, TokenValidationParameters, bool> handler = (audiences, securityToken, validationParameters) =>
            {
                // Check the Token the Back Store.
                var tokenInDb = _dbContext.OAuthTokens.FirstOrDefault(x => x.Token == token);
                if (tokenInDb == null)
                    return false;

                if (tokenInDb.Revoked)
                    return false;

                return true;
            };
            return new AudienceValidator(handler);
        }
    }
}
