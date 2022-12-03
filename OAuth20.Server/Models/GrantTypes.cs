/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using OAuth20.Server.Common;
using System.Collections.Generic;

namespace OAuth20.Server.Models
{
    public class GrantTypes
    {
        public static IList<string> Code =>
            new[] { AuthorizationGrantTypesEnum.Code.GetEnumDescription() };

        public static IList<string> Implicit =>
            new[] { AuthorizationGrantTypesEnum.Implicit.GetEnumDescription() };
        public static IList<string> ClientCredentials =>
            new[] { AuthorizationGrantTypesEnum.ClientCredentials.GetEnumDescription() };
        public static IList<string> ResourceOwnerPassword =>
            new[] { AuthorizationGrantTypesEnum.ResourceOwnerPassword.GetEnumDescription() };
    }
}
