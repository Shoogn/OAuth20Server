/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Common;
using OAuth20.Server.Configuration;
using OAuth20.Server.Helpers;
using OAuth20.Server.Models;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Models.Entities;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using OAuth20.Server.Services.CodeServce;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OAuth20.Server.Services
{
    public class AuthorizeResultService : IAuthorizeResultService
    {
        // for encrypted key see: https://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
        private readonly ClientStore _clientStore = new ClientStore();
        private readonly ICodeStoreService _codeStoreService;
        private readonly OAuthOptions _options;
        private readonly BaseDBContext _context;

        public AuthorizeResultService(ICodeStoreService codeStoreService, IOptions<OAuthOptions> options, BaseDBContext context)
        {
            _codeStoreService = codeStoreService;
            _options = options.Value;
            _context = context;
        }
        public AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest)
        {
            AuthorizeResponse response = new AuthorizeResponse();

            if (httpContextAccessor == null)
            {
                response.Error = ErrorTypeEnum.ServerError.GetEnumDescription();
                return response;
            }

            var client = VerifyClientById(authorizationRequest.client_id);
            if (!client.IsSuccess)
            {
                response.Error = client.ErrorDescription;
                return response;
            }

            if (string.IsNullOrEmpty(authorizationRequest.response_type) || authorizationRequest.response_type != "code")
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "response_type is required or is not valid";
                return response;
            }

            if (!authorizationRequest.redirect_uri.IsRedirectUriStartWithHttps() && !httpContextAccessor.HttpContext.Request.IsHttps)
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "redirect_url is not secure, MUST be TLS";
                return response;
            }


            // check the return url is match the one that in the client store


            // check the scope in the client store with the
            // one that is comming from the request MUST be matched at leaset one

            var scopes = authorizationRequest.scope.Split(' ');

            var clientScopes = from m in client.Client.AllowedScopes
                               where scopes.Contains(m)
                               select m;

            if (!clientScopes.Any())
            {
                response.Error = ErrorTypeEnum.InValidScope.GetEnumDescription();
                response.ErrorDescription = "scopes are invalids";
                return response;
            }

            string nonce = authorizationRequest.nonce;

            // Verify that a scope parameter is present and contains the openid scope value.
            // (If no openid scope value is present,
            // the request may still be a valid OAuth 2.0 request, but is not an OpenID Connect request.)

            var authoCode = new AuthorizationCode
            {
                ClientId = authorizationRequest.client_id,
                RedirectUri = authorizationRequest.redirect_uri,
                RequestedScopes = clientScopes.ToList(),
                Nonce = nonce,
                CodeChallenge = authorizationRequest.code_challenge,
                CodeChallengeMethod = authorizationRequest.code_challenege_method,
                CreationTime = DateTime.UtcNow,
                Subject = httpContextAccessor.HttpContext.User //as ClaimsPrincipal

            };

            string code = _codeStoreService.GenerateAuthorizationCode(authoCode);
            if (code == null)
            {
                response.Error = ErrorTypeEnum.TemporarilyUnAvailable.GetEnumDescription();
                return response;
            }

            response.RedirectUri = client.Client.RedirectUri + "?response_type=code" + "&state=" + authorizationRequest.state;
            response.Code = code;
            response.State = authorizationRequest.state;
            response.RequestedScopes = clientScopes.ToList();

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


        public TokenResponse GenerateToken(TokenRequest tokenRequest)
        {
            var serchBySecret = searchForClientBySecret(tokenRequest.grant_type);

            var checkClientResult = this.VerifyClientById(tokenRequest.client_id, serchBySecret, tokenRequest.client_secret);
            if (!checkClientResult.IsSuccess)
            {
                return new TokenResponse { Error = checkClientResult.Error, ErrorDescription = checkClientResult.ErrorDescription };
            }

            // Check first if the authorization_grant is client_credentials...
            // then verify the client and issued an access_token

            if (tokenRequest.grant_type == AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription())
            {
                // so generate the jwt access token
            }


            // check code from the Concurrent Dictionary
            var clientCodeChecker = _codeStoreService.GetClientDataByCode(tokenRequest.code);
            if (clientCodeChecker == null)
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };


            // check if the current client who is one made this authentication request

            if (tokenRequest.client_id != clientCodeChecker.ClientId)
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };

            // TODO: 
            // also I have to check the rediret uri 


            // Check Pkce

            if (checkClientResult.Client.UsePkce)
            {
                var pkceResult = codeVerifierIsSendByTheClientThatReceivedTheCode(tokenRequest.code_verifier,
                    clientCodeChecker.CodeChallenge, clientCodeChecker.CodeChallengeMethod);

                if (!pkceResult)
                    return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };
            }

            // Now here I will Issue the Id_token

            string id_token = string.Empty;
            if (clientCodeChecker.IsOpenId)
            {
                if (!clientCodeChecker.Subject.Identity.IsAuthenticated)
                    // I have to inform the caller to redirect the user to the login page
                    return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };

                var currentUserName = clientCodeChecker.Subject.Identity.Name;

                var userId = clientCodeChecker.Subject.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentUserName))
                    return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };

                // Generate Identity Token
                int iat = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                string[] amrs = new string[] { "pwd" };

                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();

                string publicPrivateKey = File.ReadAllText("PublicPrivateKey.xml");
                provider.FromXmlString(publicPrivateKey);

                RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);

                var claims = new List<Claim>()
                    {
                        new Claim("sub", userId),
                        new Claim("given_name", currentUserName),
                        new Claim("iat", iat.ToString(), ClaimValueTypes.Integer), // time stamp
                        new Claim("nonce", clientCodeChecker.Nonce)
                    };
                foreach (var amr in amrs)
                    claims.Add(new Claim("amr", amr));// authentication

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                var token = new JwtSecurityToken(_options.IDPUri, checkClientResult.Client.ClientId, claims,
                    expires: DateTime.UtcNow.AddMinutes(int.Parse("5")), signingCredentials: new
                    SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256));

                id_token = handler.WriteToken(token);

                var idptoken = new OAuthTokenEntity
                {
                    ClientId = checkClientResult.Client.ClientId,
                    CreationDate = DateTime.Now,
                    ReferenceId = Guid.NewGuid().ToString(),
                    Status = Constants.Statuses.Valid,
                    Token = id_token,
                    TokenType = Constants.TokenTypes.JWTIdentityToken,
                    ExpirationDate = token.ValidTo, 
                    SubjectId = userId
                };
                _context.OAuthTokens.Add(idptoken);
                _context.SaveChanges();

            }

            // Here I have to generate access token 
            var claims_at = new List<Claim>();

            var scopesinJWtAccessToken = from m in clientCodeChecker.RequestedScopes.ToList()
                                         where !OAuth2ServerHelpers.OpenIdConnectScopes.Contains(m)
                                         select m;

            foreach (var item in scopesinJWtAccessToken)
                claims_at.Add(new Claim("scope", item));

            string access_token = string.Empty;
            RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider();

            string publicPrivateKey1 = File.ReadAllText("PublicPrivateKey.xml");
            provider1.FromXmlString(publicPrivateKey1);

            RsaSecurityKey rsaSecurityKey1 = new RsaSecurityKey(provider1);
            JwtSecurityTokenHandler handler1 = new JwtSecurityTokenHandler();

            var token1 = new JwtSecurityToken(_options.IDPUri, checkClientResult.Client.ClientUri, claims_at,
                expires: DateTime.UtcNow.AddMinutes(int.Parse("5")), signingCredentials: new
                SigningCredentials(rsaSecurityKey1, SecurityAlgorithms.RsaSha256));

            access_token = handler1.WriteToken(token1);


            var atoken = new OAuthTokenEntity
            {
                ClientId = checkClientResult.Client.ClientId,
                CreationDate = DateTime.Now,
                ReferenceId = Guid.NewGuid().ToString(),
                Status = Constants.Statuses.Valid,
                Token = access_token,
                TokenType = Constants.TokenTypes.JWTAcceseccToken,
                ExpirationDate = token1.ValidTo
            };

            _context.OAuthTokens.Add(atoken);
            _context.SaveChanges();

            // here remove the code from the Concurrent Dictionary
            _codeStoreService.RemoveClientDataByCode(tokenRequest.code);

            return new TokenResponse
            {
                access_token = access_token,
                id_token = id_token,
                code = tokenRequest.code
            };
        }

        private bool codeVerifierIsSendByTheClientThatReceivedTheCode(string codeVerifier, string codeChallenge, string codeChallengeMethod)
        {
            var odeVerifireAsByte = Encoding.ASCII.GetBytes(codeVerifier);

            if (codeChallengeMethod == Constants.Plain)
            {
                using var shaPalin = SHA256.Create();
                var computedHashPalin = shaPalin.ComputeHash(odeVerifireAsByte);
                var tranformedResultPalin = Base64UrlEncoder.Encode(computedHashPalin);
                return tranformedResultPalin.Equals(codeChallenge);
            }

            using var shaS256 = SHA256.Create();
            var computedHashS256 = shaS256.ComputeHash(odeVerifireAsByte);
            var tranformedResultS256 = Base64UrlEncoder.Encode(computedHashS256);

            return tranformedResultS256.Equals(codeChallenge);
        }


        private bool searchForClientBySecret(string grantType)
        {
            if (grantType == AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription() ||
                grantType == AuthorizationGrantTypesEnum.RefreshToken.GetEnumDescription())
                return false;

            return true;
        }
    }
}
