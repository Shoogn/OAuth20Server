/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

namespace OAuth20.Server.Enumeration;

/// <summary>
/// For more information see the <see cref="https://www.rfc-editor.org/info/rfc6750"/>
/// </summary>
public enum BearerTokenUsageTypeEnum : byte
{
    AuthorizationRequestHeader,
    FormEncodedBodyParameter,
    UriQueryParameter
}
