namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    /// <summary>
    /// Define default value to use in the <see cref="OAuth2ServerHandler"/> for JWT bearer authentication.
    /// </summary>
    public static class OAuth2IntrospectionJwtBearerDefaults
    {
        /// <summary>
        /// The default authentication scheme.
        /// </summary>
        public const string AuthenticationScheme = "Bearer";

        /// <summary>
        /// HttpClient name, that will be resolved from HttpClientFactory.
        /// </summary>
        public const string NamedBackChannelHttpClient = "OAuth2BackChannelHttpClient";
    }
}
