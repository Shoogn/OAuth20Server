using Microsoft.AspNetCore.Authentication;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    public static class OAuth2JwtBearerExtensions
    {
        public static AuthenticationBuilder AddOAuth2IntrospectionJwtBearer(this AuthenticationBuilder builder, 
            string authenticationSchem,Action<OAuth2ServerOptions> configureOptions)
        {
            builder.Services.AddHttpClient(OAuth2IntrospectionJwtBearerDefaults.NamedBackChannelHttpClient);

            // The configureOptions will registerd by defult internaly if is not null in a method named AddSchemeHelper( ... parameters here ... );

            return builder.AddScheme< OAuth2ServerOptions, OAuth2ServerHandler>(authenticationSchem, configureOptions);
        }
    }
}
