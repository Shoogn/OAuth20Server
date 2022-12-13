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
    /// <summary>
    /// This is an APIs applications
    /// </summary>
    public class ProtectedResource
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> AllowScopes { get; set; }
    }
}
