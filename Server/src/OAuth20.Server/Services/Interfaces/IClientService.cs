/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using OAuth20.Server.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OAuth20.Server.Services
{
    public interface IClientService
    {
        CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = null,
            string grantType = null);

        bool SearchForClientBySecret(string grantType);
        AudienceValidator ValidateAudienceHandler(IEnumerable<string> audiences, SecurityToken securityToken,
            TokenValidationParameters validationParameters, Client client, string token);

        Task<CheckClientResult> GetClientByUriAsync(string clientUrl);

        Task<CheckClientResult> GetClientByIdAsync(string clientId);
    }
}
