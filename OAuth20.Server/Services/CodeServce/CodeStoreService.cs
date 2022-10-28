using Microsoft.AspNetCore.Authentication.Cookies;
using OAuth20.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OAuth20.Server.Services.CodeServce
{
    public class CodeStoreService : ICodeStoreService
    {
        private readonly ConcurrentDictionary<string, AuthorizationCode> _codeIssued = new ConcurrentDictionary<string, AuthorizationCode>();
        private readonly ClientStore _clientStore = new ClientStore();

        // Here I genrate the code for authorization, and I will store it 
        // in the Concurrent Dictionary

        public string GenerateAuthorizationCode(string clientId, IList<string> requestedScope)
        {
            var client = _clientStore.Clients.Where(x => x.ClientId == clientId).FirstOrDefault();

            if(client != null)
            {
                var code = Guid.NewGuid().ToString();

                var authoCode = new AuthorizationCode
                {
                    ClientId = clientId,
                    RedirectUri = client.RedirectUri,
                    RequestedScopes = requestedScope,
                };

                // then store the code is the Concurrent Dictionary
                _codeIssued[code] = authoCode;

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
        public AuthorizationCode UpdatedClientDataByCode(string key, IList<string> requestdScopes,
            string userName, string password = null, string nonce = null)
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
                        Nonce = nonce
                    };


                    // ------------------ I suppose the user name and password is correct  -----------------
                    var claims = new List<Claim>();



                    if (newValue.IsOpenId)
                    {
                        // TODO
                        // Add more claims to the claims
                    
                    }

                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    newValue.Subject = new ClaimsPrincipal(claimIdentity);
                    // ------------------ -----------------------------------------------  -----------------

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
            _codeIssued.TryRemove(key, out authorizationCode);
            return null;
        }
    }
}
