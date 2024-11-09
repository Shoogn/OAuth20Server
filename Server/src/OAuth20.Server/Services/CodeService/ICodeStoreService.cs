/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using OAuth20.Server.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace OAuth20.Server.Services.CodeService
{
    public interface ICodeStoreService
    {
        Task<string> GenerateAuthorizationCodeAsync(AuthorizationCode authorizationCode);
        Task<AuthorizationCode> GetClientDataByCodeAsync(string key);
        Task<AuthorizationCode> UpdatedClientDataByCodeAsync(string key, ClaimsPrincipal claimsPrincipal, IList<string> requestdScopes);
        Task RemoveClientDataByCodeAsync(string key);
    }
}
