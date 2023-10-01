using Microsoft.AspNetCore.Authentication;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    public class AuthenticationFailedContext : ResultContext<OAuth2ServerOptions>
    {
        public AuthenticationFailedContext(HttpContext context,
            AuthenticationScheme scheme, 
            OAuth2ServerOptions options) 
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Error message 
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
