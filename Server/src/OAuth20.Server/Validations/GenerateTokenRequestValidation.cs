using OAuth20.Server.Common;
using OAuth20.Server.Enumeration;
using OAuth20.Server.Models;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Services.Grants;
using OAuth20.Server.Validations.Interfaces;
using OAuth20.Server.Validations.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth20.Server.Validations
{
    /// <summary>
    /// Checks the shape of a token request: known grant type and the parameters that grant requires.
    /// Client authentication and grant specific rules are checked by the grant handlers.
    /// </summary>
    public class GenerateTokenRequestValidation : IGenerateTokenRequestValidation
    {
        private readonly HashSet<string> _supportedGrantTypes;

        public GenerateTokenRequestValidation(IEnumerable<IGrantHandler> grantHandlers)
        {
            _supportedGrantTypes = grantHandlers.Select(x => x.GrantType).ToHashSet();
        }

        public Task<GenerateTokenValidationResponse> ValidateAsync(TokenRequest tokenRequest)
        {
            var invalidRequest = ErrorTypeEnum.InvalidRequest.GetEnumDescription();

            GenerateTokenValidationResponse Fail(string error, string description) =>
                new GenerateTokenValidationResponse { Error = error, ErrorDescription = description };

            if (tokenRequest == null)
                return Task.FromResult(Fail(invalidRequest, "request body is required"));

            if (string.IsNullOrWhiteSpace(tokenRequest.grant_type))
                return Task.FromResult(Fail(invalidRequest, "grant_type is required"));

            if (!_supportedGrantTypes.Contains(tokenRequest.grant_type))
                return Task.FromResult(Fail("unsupported_grant_type", "grant_type is not supported"));

            // client_credentials sends the client in the Basic Authorization header
            if (tokenRequest.grant_type != AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription()
                && string.IsNullOrWhiteSpace(tokenRequest.client_id))
                return Task.FromResult(Fail(invalidRequest, "client_id is required"));

            string missing = null;
            if (tokenRequest.grant_type == AuthorizationGrantTypesEnum.AuthorizationCode.GetEnumDescription())
            {
                if (string.IsNullOrWhiteSpace(tokenRequest.code)) missing = "code";
            }
            else if (tokenRequest.grant_type == AuthorizationGrantTypesEnum.DeviceCode.GetEnumDescription())
            {
                if (string.IsNullOrWhiteSpace(tokenRequest.device_code)) missing = "device_code";
            }
            else if (tokenRequest.grant_type == AuthorizationGrantTypesEnum.TokenExchange.GetEnumDescription())
            {
                if (string.IsNullOrWhiteSpace(tokenRequest.subject_token)) missing = "subject_token";
                else if (string.IsNullOrWhiteSpace(tokenRequest.subject_token_type)) missing = "subject_token_type";
                else if (tokenRequest.subject_token_type != TokenExchangeGrantHandler.AccessTokenType)
                    return Task.FromResult(Fail(invalidRequest, "subject_token_type is not supported"));
                else if (!string.IsNullOrEmpty(tokenRequest.actor_token) && tokenRequest.actor_token_type != TokenExchangeGrantHandler.AccessTokenType)
                    return Task.FromResult(Fail(invalidRequest, "actor_token_type is missing or not supported"));
                else if (!string.IsNullOrEmpty(tokenRequest.requested_token_type) && tokenRequest.requested_token_type != TokenExchangeGrantHandler.AccessTokenType)
                    return Task.FromResult(Fail(invalidRequest, "requested_token_type is not supported"));
            }

            if (missing != null)
                return Task.FromResult(Fail(invalidRequest, $"{missing} is required"));

            return Task.FromResult(new GenerateTokenValidationResponse { Succeeded = true });
        }
    }
}
