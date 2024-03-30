﻿namespace OAuth20.Server
{
    public static class Constants
    {
        public static class ChallengeMethod
        {
            public const string Plain = "plain";
            public const string SHA256 = "S256";
        }

        public static class OpenIdConnectScopes
        {
            public const string OpenId = "openid";
            public const string Profile = "profile";
            public const string Email = "email";
            public const string Address = "address";
            public const string Phone = "phone";
            public const string OfflineAccess = "offline_access";
        }

        public static class TokenTypes
        {
            public const string JWTAcceseccToken = "Access_Token";
            public const string JWTIdentityToken = "Idp_Token";
            public const string AccessAndIdpToken = "Access_Idp_Token";
        }


        public static class TokenTypeHints
        {
            public const string AccessToken = "access_token";
            public const string RefreshToken = "refresh_token";
        }

        public static class Statuses
        {
            public const string InActive = "inactive";
            public const string Revoked = "revoked";
            public const string Valid = "valid";
        }

        public static class ContentTypeSupported
        {
            public const string XwwwFormUrlEncoded = "application/x-www-form-urlencoded";
        }

        public static class AuthenticatedRequestScheme
        {
            public const string AuthorizationRequestHeader = "Bearer";
            public const string FormEncodedBodyParameter = "access_token";
            public const string UriQueryParameter = "access_token";
        }
    }
}
