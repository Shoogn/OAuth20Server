﻿/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using OAuth20.Server.Validations;

namespace OAuth20.Server.Services
{
    public class DeviceAuthorizationService : IDeviceAuthorizationService
    {
        private readonly IDeviceAuthorizationValidation _validation;
        public DeviceAuthorizationService(IDeviceAuthorizationValidation validation)
        {
            _validation = validation;
        }



    }
}
