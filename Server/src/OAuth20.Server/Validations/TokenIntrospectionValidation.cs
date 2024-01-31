﻿/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Http;
using OAuth20.Server.Models;
using OAuth20.Server.OauthResponse;
using System.Linq;
using System;
using System.Threading.Tasks;
using OAuth20.Server.Common;
using System.Text;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Validations.Response;
using System.Collections;

namespace OAuth20.Server.Validations
{
    public class TokenIntrospectionValidation : ITokenIntrospectionValidation
    {
        private readonly ClientStore _clientStore = new ClientStore();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenIntrospectionValidation(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public virtual Task<TokenIntrospectionValidationResponse> ValidateAsync(TokenIntrospectionRequest tokenIntrospectionRequest)
        {
            var response = new TokenIntrospectionValidationResponse() { Succeeded = true };
            var context = _httpContextAccessor.HttpContext;
            var requestContentType = context.Request.ContentType;
            if (!requestContentType.Equals(Constants.ContentTypeSupported.XwwwFormUrlEncoded))
            {
                response.Succeeded = false;
                response.Error = "Content Type is not supported";
                return Task.FromResult(response);
            }

            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (authorizationHeader == null)
            {
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return Task.FromResult(response);
            }
            if (!authorizationHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
            {
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return Task.FromResult(response);
            }
            try
            {
                string parameters = authorizationHeader.Substring("Basic ".Length);
                string authorizationKeys = Encoding.UTF8.GetString(Convert.FromBase64String(parameters));

                int authorizationResult = authorizationKeys.IndexOf(':');
                if (authorizationResult == -1)
                {
                    response.Succeeded = false;
                    response.Error = "Client is not Authorized";
                    return Task.FromResult(response);
                }
                string clinetId = authorizationKeys.Substring(0, authorizationResult);
                string clientSecret = authorizationKeys.Substring(authorizationResult + 1);

                // Here I have to get the client from the Client Store
                CheckClientResult client = VerifyClientById(clinetId, true, clientSecret);
                if (!client.IsSuccess)
                {
                    response.Succeeded = false;
                    response.Error = "Client is not Authorized";
                    return Task.FromResult(response);
                }

                response.Client = client.Client;
            }
            catch (Exception ex)
            {
                _ = ex;
                response.Succeeded = false;
                response.Error = "Client is not Authorized";
                return Task.FromResult(response);
            }
            return Task.FromResult(response);

        }



        private CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = null)
        {
            CheckClientResult result = new CheckClientResult() { IsSuccess = false };

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = _clientStore.Clients
                    .Where(x => x.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (client != null)
                {

                    // I have to check is the client has ClientCredentials Grant Type
                    //var clientGrantTypes = client.GrantTypes;
                    //bool clientGrantTypesCheckResult = clientGrantTypes.Contains(AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription());
                    //if(clientGrantTypesCheckResult == false)
                    //{
                    //    result.Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription();
                    //    return result;
                    //}


                    if (checkWithSecret && !string.IsNullOrEmpty(clientSecret))
                    {
                        bool hasSameSecretId = client.ClientSecret.Equals(clientSecret, StringComparison.InvariantCulture);
                        if (!hasSameSecretId)
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
                        result.ErrorDescription = ErrorTypeEnum.UnAuthorizedClient.GetEnumDescription();
                        return result;
                    }
                }
            }

            result.ErrorDescription = ErrorTypeEnum.AccessDenied.GetEnumDescription();
            return result;
        }
    }
}
