/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Common;
using OAuth20.Server.Configuration;
using OAuth20.Server.Enumeration;
using OAuth20.Server.Models;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Models.Entities;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using OAuth20.Server.Services.CodeService;
using OAuth20.Server.Services.Grants;
using OAuth20.Server.Services.Interfaces;
using OAuth20.Server.Validations.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OAuth20.Server.Services
{
    public class AuthorizeRequestService : IAuthorizeResultService
    {
        // for encrypted key see: https://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
        private readonly ICodeStoreService _codeStoreService;
        private readonly IClientService _clientService;
        private readonly IEnumerable<IGrantHandler> _grantHandlers;
        private readonly IGenerateTokenRequestValidation _generateTokenRequestValidation;

        public AuthorizeRequestService(ICodeStoreService codeStoreService,
            IClientService clientService,
            IEnumerable<IGrantHandler> grantHandlers,
            IGenerateTokenRequestValidation generateTokenRequestValidation)
        {
            _codeStoreService = codeStoreService;
            _clientService = clientService;
            _grantHandlers = grantHandlers;
            _generateTokenRequestValidation = generateTokenRequestValidation;
        }
        public async Task<AuthorizeResponse> AuthorizeRequestAsync(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest)
        {
            AuthorizeResponse response = new AuthorizeResponse();

            if (httpContextAccessor == null)
            {
                response.Error = ErrorTypeEnum.ServerError.GetEnumDescription();
                return response;
            }

            var client = _clientService.VerifyClientById(authorizationRequest.ClientId);
            if (!client.IsSuccess)
            {
                response.Error = client.Error;
                response.ErrorDescription = client.ErrorDescription;
                return response;
            }

            if (string.IsNullOrEmpty(authorizationRequest.ResponseType) || authorizationRequest.ResponseType != "code")
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "response type is required or is not valid";
                return response;
            }

            if (!authorizationRequest.Redirecturi.IsRedirectUriStartWithHttps() && !httpContextAccessor.HttpContext.Request.IsHttps)
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "redirect url is not secure, MUST be TLS";
                return response;
            }

            if (client.Client.UsePkce && string.IsNullOrWhiteSpace(authorizationRequest.CodeChallenge))
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "code challenge required";
                return response;

            }
            // check the return url is match the one that in the client store

            var uri = client.Client.RedirectUris
                .Where(x => x.Contains(authorizationRequest.Redirecturi, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(uri))
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "redirect uri is not matched the one in the client store";
                return response;
            }

            // check the scope in the client store with the
            // one that is comming from the request MUST be matched at leaset one

            var scopes = authorizationRequest.Scope.Split(' ');

            var clientScopes = from m in client.Client.AllowedScopes
                               where scopes.Contains(m)
                               select m;

            if (!clientScopes.Any())
            {
                response.Error = ErrorTypeEnum.InValidScope.GetEnumDescription();
                response.ErrorDescription = "scopes are invalids";
                return response;
            }

            string nonce = authorizationRequest.Nonce;

            // Verify that a scope parameter is present and contains the openid scope value.
            // (If no openid scope value is present,
            // the request may still be a valid OAuth 2.0 request, but is not an OpenID Connect request.)

            var authoCode = new AuthorizationCode
            {
                ClientId = authorizationRequest.ClientId,
                RedirectUri = authorizationRequest.Redirecturi,
                RequestedScopes = clientScopes.ToList(),
                Nonce = nonce,
                CodeChallenge = authorizationRequest.CodeChallenge,
                CodeChallengeMethod = authorizationRequest.CodeChallengeMethod,
                CreationTime = DateTime.UtcNow,
                Subject = httpContextAccessor.HttpContext.User //as ClaimsPrincipal

            };

            string code = await _codeStoreService.GenerateAuthorizationCodeAsync(authoCode);
            if (code == null)
            {
                response.Error = ErrorTypeEnum.TemporarilyUnAvailable.GetEnumDescription();
                return response;
            }

            Dictionary<string, string> qs = new Dictionary<string, string>
            {
                { "response_type", "code" },
                { "state", authorizationRequest.State }
            };


            response.RedirectUri = uri + QueryString.Create(qs);
            response.Code = code;
            response.State = authorizationRequest.State;
            response.RequestedScopes = clientScopes.ToList();

            return response;
        }

        public async Task<TokenResponse> GenerateTokenAsync(TokenRequest tokenRequest)
        {
            var validation = await _generateTokenRequestValidation.ValidateAsync(tokenRequest);
            if (!validation.Succeeded)
                return new TokenResponse { Error = validation.Error, ErrorDescription = validation.ErrorDescription };

            var handler = _grantHandlers.FirstOrDefault(x => x.GrantType == tokenRequest.grant_type);
            if (handler == null)
                return new TokenResponse { Error = "unsupported_grant_type", ErrorDescription = "grant_type is not supported" };

            return await handler.HandleAsync(tokenRequest);
        }
    }
}
