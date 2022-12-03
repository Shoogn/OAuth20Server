using OAuth20.Server.Models;
using System.Collections.Generic;

namespace OAuth20.Server.Services.CodeServce
{
    public interface ICodeStoreService
    {
        string GenerateAuthorizationCode(AuthorizationCode authorizationCode);
        AuthorizationCode GetClientDataByCode(string key);
        AuthorizationCode UpdatedClientDataByCode(string key, IList<string> requestdScopes);
        AuthorizationCode RemoveClientDataByCode(string key);
    }
}
