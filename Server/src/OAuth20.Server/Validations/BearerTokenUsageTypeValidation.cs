/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Http;
using OAuth20.Server.Validations.Response;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Validations
{
    public class BearerTokenUsageTypeValidation : IBearerTokenUsageTypeValidation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BearerTokenUsageTypeValidation(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<BearerTokenUsageTypeValidationResponse> ValidateAsync()
        {
            var response = new BearerTokenUsageTypeValidationResponse { Succeeded = false };
            //("Authorization", out var token)
            var authroizationHeader = _httpContextAccessor.HttpContext.Request.Headers.Authorization;

            var header = authroizationHeader.First().Trim();
            if (header is not null && header.StartsWith(Constants.AuthenticatedRequestScheme.AuthorizationRequestHeader))
            {
                var token = header.Substring(Constants.AuthenticatedRequestScheme.AuthorizationRequestHeader.Length).Trim();
                if (token is not null && token.Length > 0)
                {
                    response.Succeeded = true;
                    response.Token = token;
                    response.BearerTokenUsageType = Enumeration.BearerTokenUsageTypeEnum.AuthorizationRequestHeader;
                    return Task.FromResult(response);
                }
            }

            var postForm = _httpContextAccessor.HttpContext.Request.Form;
            if(postForm != null && postForm.Any())
            {
                if (postForm.ContainsKey(Constants.AuthenticatedRequestScheme.FormEncodedBodyParameter))
                {
                    var token = postForm.Where(x=>x.Key == Constants.AuthenticatedRequestScheme.AuthorizationRequestHeader)
                        .Select(x=>x.Value).First().ToString();
                    if(token is not null)
                    {
                        response.Succeeded = true;
                        response.Token = token;
                        response.BearerTokenUsageType = Enumeration.BearerTokenUsageTypeEnum.FormEncodedBodyParameter;
                        return Task.FromResult(response);
                    }
                }
            }


            return Task.FromResult(response);
        }
    }
}
