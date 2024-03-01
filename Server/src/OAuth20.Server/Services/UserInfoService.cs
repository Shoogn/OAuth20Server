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
using OAuth20.Server.OAuthResponse;
using OAuth20.Server.Services.Users;
using OAuth20.Server.Validations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OAuth20.Server.Services;

public interface IUserInfoService
{
    Task<UserInfoResponse> GetUserInfoAsync();
}
public class UserInfoService : IUserInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBearerTokenUsageTypeValidation _bearerTokenUsageTypeValidation;
    private readonly OAuthServerOptions _optionsMonitor;
    private readonly IClientService _clientService;
    private readonly IUserManagerService _userManagerService;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(IHttpContextAccessor httpContextAccessor,
         IBearerTokenUsageTypeValidation bearerTokenUsageTypeValidation,
         IOptionsMonitor<OAuthServerOptions> optionsMonitor,
         IClientService clientService,
         IUserManagerService userManagerService,
         ILogger<UserInfoService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _bearerTokenUsageTypeValidation = bearerTokenUsageTypeValidation;
        _optionsMonitor = optionsMonitor.CurrentValue ?? new OAuthServerOptions();
        _clientService = clientService;
        _userManagerService = userManagerService;
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
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            string publicPrivateKey = File.ReadAllText("PublicPrivateKey.xml");
            provider.FromXmlString(publicPrivateKey);
            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(bearerTokenUsages.Token);

            var clientId = jwtSecurityToken.Audiences.FirstOrDefault();
            var client = await _clientService.GetClientByIdAsync(clientId);

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
                    string userId = tokenValidationReslt.ClaimsIdentity.FindFirst("sub")?.Value;

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
