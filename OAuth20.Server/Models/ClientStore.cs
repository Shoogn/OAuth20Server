/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using System.Collections.Generic;

namespace OAuth20.Server.Models
{
    public class ClientStore
    {
        public IEnumerable<Client> Clients = new[]
        {
            new Client
            {
                ClientName = "blazorWasm",
                ClientId = "1",
                ClientSecret = "123456789",
                AllowedScopes = new[]{ "openid", "profile", "blazorWasmapi.readandwrite" },
                GrantTypes = GrantTypes.Code,
                IsActive = false,
                ClientUri = "https://localhost:7026",
                RedirectUri = "https://localhost:7026/signin-oidc",
                UsePkce = true,
            },
            new Client
            {
                ClientName = "openIdtestapp",
                ClientId = "2",
                ClientSecret = "123456789",
                AllowedScopes = new[]{ "openid", "profile", "jwtapitestapp.read", "jwtapitestapp.readandwrite" },
                GrantTypes = GrantTypes.CodeAndClientCredentials,
                IsActive = true,
                ClientUri = "https://localhost:7276",
                RedirectUri = "https://localhost:7276/signin-oidc",
                UsePkce = true,
            },
              new Client
            {
                ClientName = "jwtapitestapp",
                ClientId = "3",
                ClientSecret = "123456789",
                AllowedScopes = new[]{ "" },
                GrantTypes = GrantTypes.ClientCredentials,
                IsActive = true,
                ClientUri = "https://localhost:7065",
            }
        };
    }
}
