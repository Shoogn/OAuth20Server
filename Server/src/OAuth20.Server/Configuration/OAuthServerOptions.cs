/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

namespace OAuth20.Server.Configuration
{
    public class OAuthServerOptions
    {
        /// <summary>
        /// This indicate which provider our identity provider will use
        /// InMemoey Or BackStore
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// If is not availabe so, no user can login or register
        /// This will shout down the application domain
        /// </summary>
        public bool IsAvaliable { get; set; }

        /// <summary>
        /// This is the uri of the identity provider
        /// </summary>
        public string IDPUri { get; set; }

        /// <summary>
        /// Get or set the device flow interval value in seconds to let client wait before
        /// calling the Token endpoint repeatedly. <c>Default value is 5 seconds</c>
        /// </summary>
        public int DeviceFlowInterval { get; set; } = 5;

    }
}
