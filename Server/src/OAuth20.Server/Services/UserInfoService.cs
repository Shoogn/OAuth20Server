/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Configuration;
using OAuth20.Server.Infrastructure;
using OAuth20.Server.OAuthResponse;
using OAuth20.Server.Services.Interfaces;
using OAuth20.Server.Services.Users;
using OAuth20.Server.Validations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OAuth20.Server.Services;

public interface IUserInfoService
{
    Task<UserInfoResponse> GetUserInfoAsync();
}
public class UserInfoService : IUserInfoService
{
    private readonly IBearerTokenUsageTypeValidation _bearerTokenUsageTypeValidation;
    private readonly OAuthServerOptions _optionsMonitor;
    private readonly IClientService _clientService;
    private readonly IUserManagerService _userManagerService;
    private readonly ISigningKeyStore _signingKeyStore;
    private readonly IOAuth2JwtDecoder _oAuth2JwtDecoder;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(
         IBearerTokenUsageTypeValidation bearerTokenUsageTypeValidation,
         IOptionsMonitor<OAuthServerOptions> optionsMonitor,
         IClientService clientService,
         IUserManagerService userManagerService,
         ISigningKeyStore signingKeyStore,
         IOAuth2JwtDecoder oAuth2JwtDecoder,
         ILogger<UserInfoService> logger)
    {
        _bearerTokenUsageTypeValidation = bearerTokenUsageTypeValidation;
        _optionsMonitor = optionsMonitor.CurrentValue ?? new OAuthServerOptions();
        _clientService = clientService;
        _userManagerService = userManagerService;
        _signingKeyStore = signingKeyStore;
        _oAuth2JwtDecoder = oAuth2JwtDecoder;
        _logger = logger;
    }

    public async Task<UserInfoResponse> GetUserInfoAsync()
    {
        var response = new UserInfoResponse();
        var bearerTokenUsages = await _bearerTokenUsageTypeValidation.ValidateAsync();
        if (bearerTokenUsages.Succeeded == false)
        {

            response.Claims = null;
            response.Succeeded = false;
            response.Error = "no token found";
            response.ErrorDescription = "Make sure to add the token as bearer to Authentication header in the request";
        }
        else
        {
            // validate to token
            var decodeToken = _oAuth2JwtDecoder.Decode(bearerTokenUsages.Token);
            RSA publicKey = _signingKeyStore.GetPublicKey(decodeToken.Header.Kid);
            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(publicKey);


            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(bearerTokenUsages.Token);

            var aud = jwtSecurityToken.Audiences.FirstOrDefault();
            var client = await _clientService.GetClientByUriAsync(aud);
            
            // TODO:
            // check if client is null.
            // check if client is not active.

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters();

            tokenValidationParameters.IssuerSigningKey = rsaSecurityKey;
            tokenValidationParameters.ValidAudiences = jwtSecurityToken.Audiences;
            tokenValidationParameters.ValidTypes = new[] { "JWT" };
            tokenValidationParameters.ValidateIssuer = true;
            tokenValidationParameters.ValidIssuer = _optionsMonitor.IDPUri;
            tokenValidationParameters.ValidateAudience = true;
            tokenValidationParameters.AudienceValidator = _clientService.ValidateAudienceHandler(
                jwtSecurityToken.Audiences, jwtSecurityToken,
                tokenValidationParameters, client.Client, bearerTokenUsages.Token);

            try
            {
                var tokenValidationReslt = await jwtSecurityTokenHandler.ValidateTokenAsync(bearerTokenUsages.Token, tokenValidationParameters);

                if (tokenValidationReslt.IsValid)
                {
                    var payload = jwtSecurityToken.Payload;
                    //string userId = tokenValidationReslt.ClaimsIdentity.FindFirst("sub")?.Value;
                    var userId = payload.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                    // TODO:
                    // check userId is null

                    var user = await _userManagerService.GetUserAsync(userId);
                    // TODO:
                    // check user is null


                    // here build the response as json

                    string scope = (tokenValidationReslt.Claims.FirstOrDefault(x => x.Key == "scope").Value).ToString();

                    response.Sub = userId;
                    response.EmailVerified = user.EmailConfirmed;
                    response.Email = user.Email;
                    response.Name = user.Email;

                    //response.Active = true;
                    //response.TokenType = "access_token";
                    //response.Exp = exp;
                    //response.Iat = (int)jwtSecurityToken.IssuedAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    //response.Iss = _optionsMonitor.IDPUri;
                    //response.Scope = scope;
                    //response.Aud = aud;
                    //response.Nbf = (int)jwtSecurityToken.IssuedAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                }
            }
            catch (Exception ex) // maybe SecurityTokenException
            {
                _logger.LogCritical("There is an exception during fetching the user info, {exception}", ex);
                response.Claims = null;
                response.Succeeded = false;
                response.Error = "invalid_token";
                response.ErrorDescription = "token is not valid";
            }


        }

        return response;
    }
}
