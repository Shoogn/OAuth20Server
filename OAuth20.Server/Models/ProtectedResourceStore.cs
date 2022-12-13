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
    public class ProtectedResourceStore
    {
        public IEnumerable<ProtectedResource> ProtectedResources = new[]
        {
            new ProtectedResource()
            {
                Name = "blazorWasmapi",
                Description = "this is an blazor wasm api application",
                AllowScopes = new[] { "blazorWasmapi.readpost", "blazorWasmapi.writepost", "blazorWasmapi.readandwrite" }
            }
        };
    }
}
