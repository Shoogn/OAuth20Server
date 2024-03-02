namespace OAuth20.Server
{
    public static class Constants
    {
        public static class ChallengeMethod
        {
            public static string Plain = "plain";
            public static string SHA256 = "S256";
        }

        public static class OpenIdConnectScopes
        {
            public static string OpenId = "openid";
            public static string Profile = "profile";
            public static string Email = "email";
            public static string Address = "address";
            public static string Phone = "phone";
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
