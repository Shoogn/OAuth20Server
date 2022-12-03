using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;

namespace OAuth20.Server.Services.CodeServce
{
    public class CodeStoreService : ICodeStoreService
    {
        private readonly ConcurrentDictionary<string, AuthorizationCode> _codeIssued = new ConcurrentDictionary<string, AuthorizationCode>();
        private readonly ClientStore _clientStore = new ClientStore();

        // Here I genrate the code for authorization, and I will store it 
        // in the Concurrent Dictionary

        public string GenerateAuthorizationCode(AuthorizationCode authorizationCode)
        {
            var client = _clientStore.Clients.Where(x => x.ClientId == authorizationCode.ClientId).SingleOrDefault();

            if (client != null)
            {
                var rand = RandomNumberGenerator.Create();
                byte[] bytes = new byte[32];
                rand.GetBytes(bytes);
                var code = Base64UrlEncoder.Encode(bytes);

                _codeIssued[code] = authorizationCode;

                return code;
            }
            return null;
        }

        public AuthorizationCode GetClientDataByCode(string key)
        {
            AuthorizationCode authorizationCode;
            if (_codeIssued.TryGetValue(key, out authorizationCode))
            {
                return authorizationCode;
            }
            return null;
        }

        // TODO
        // Before updated the Concurrent Dictionary I have to Process User Sign In,
        // and check the user credienail first
        // But here I merge this process here inside update Concurrent Dictionary method
        public AuthorizationCode UpdatedClientDataByCode(string key, IList<string> requestdScopes)
        {
            var oldValue = GetClientDataByCode(key);

            if (oldValue != null)
            {
                // check the requested scopes with the one that are stored in the Client Store 
                var client = _clientStore.Clients.Where(x => x.ClientId == oldValue.ClientId).FirstOrDefault();

                if (client != null)
                {
                    var clientScope = (from m in client.AllowedScopes
                                       where requestdScopes.Contains(m)
                                       select m).ToList();

                    if (!clientScope.Any())
                        return null;

                    AuthorizationCode newValue = new AuthorizationCode
                    {
                        ClientId = oldValue.ClientId,
                        CreationTime = oldValue.CreationTime,
                        IsOpenId = requestdScopes.Contains("openId") || requestdScopes.Contains("profile"),
                        RedirectUri = oldValue.RedirectUri,
                        RequestedScopes = requestdScopes,
                        Nonce = oldValue.Nonce,
                        CodeChallenge = oldValue.CodeChallenge,
                        CodeChallengeMethod = oldValue.CodeChallengeMethod,
                        Subject = oldValue.Subject,
                    };
                    var result = _codeIssued.TryUpdate(key, newValue, oldValue);

                    if (result)
                        return newValue;
                    return null;
                }
            }
            return null;
        }

        public AuthorizationCode RemoveClientDataByCode(string key)
        {
            AuthorizationCode authorizationCode;
            var isRemoved = _codeIssued.TryRemove(key, out authorizationCode);
            if (isRemoved)
                return authorizationCode;
            return null;
        }
    }
}
