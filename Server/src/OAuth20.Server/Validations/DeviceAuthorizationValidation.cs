/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */


using Microsoft.AspNetCore.Http;
using OAuth20.Server.Common;
using OAuth20.Server.Models;
using OAuth20.Server.Services;
using OAuth20.Server.Validations.Response;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Validations
{
    public class DeviceAuthorizationValidation : IDeviceAuthorizationValidation
    {
        private readonly IClientService _clientService;
        public DeviceAuthorizationValidation(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<DeviceAuthorizationValidationResponse> ValidateAsync(HttpContext httpContext)
        {
            var response = new DeviceAuthorizationValidationResponse() { Succeeded = false };


            if (httpContext is null)
            {
                response.ErrorDescription = "Httpcontext is null, please check the request";
                return response;
            }

            // validate the method it should be http post
            if (httpContext.Request.Method != HttpMethods.Post)
            {
                response.ErrorDescription = "The http request method is not valid";
                return response;
            }

            // validate the content type it should be form url encoded
            if (!httpContext.Request.ContentType?.Equals(Constants.ContentTypeSupported.XwwwFormUrlEncoded, System.StringComparison.OrdinalIgnoreCase) ?? false)
            {
                response.ErrorDescription = "The http request content type is not valid";
                return response;
            }

            // get the client id & the scope from the form
            var clientId = httpContext.Request.Form["client_id"];

            // check the client 
            var client = await _clientService.GetClientByIdAsync(clientId);
            if (client.IsSuccess)
            {
                var clientVerifiedResult = _clientService.VerifyClientById(clientId, checkWithSecret: true, client.Client.ClientSecret);

                if (clientVerifiedResult.IsSuccess)
                {
                    response.Client = clientVerifiedResult.Client;

                    // check grant types
                    if (!clientVerifiedResult.Client.GrantTypes.Contains(AuthorizationGrantTypesEnum.DeviceCode.GetEnumDescription()))
                    {
                        response.ErrorDescription = "invalid grant type, check that the client support the device_code";
                        return response;
                    }

                    // TODO:
                    // check the scopes
                    var scopes = httpContext.Request.Form["scope"];
                    if (!scopes.Any())
                    {
                        // check scopes from the client store
                    }

                }
                else
                {
                    response.ErrorDescription = clientVerifiedResult.ErrorDescription;
                    return response;
                }
            }

            response.ErrorDescription = "Unauthorized client";
            return response;
        }
    }
}
