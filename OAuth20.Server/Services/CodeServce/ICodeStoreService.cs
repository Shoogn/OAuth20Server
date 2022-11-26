using OAuth20.Server.Models;
using System.Collections.Generic;

namespace OAuth20.Server.Services.CodeServce
{
    public interface ICodeStoreService
    {
        string GenerateAuthorizationCode(string clientId, string nonce, IList<string> requestedScope);
        AuthorizationCode GetClientDataByCode(string key);
        AuthorizationCode UpdatedClientDataByCode(string key, IList<string> requestdScopes, string userName, string password = null);
        AuthorizationCode RemoveClientDataByCode(string key);
    }
}
