using Microsoft.AspNetCore.Authentication;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    /// <summary>
    /// Define the context for validated token.
    /// </summary>
    public class TokenValidatedContext : ResultContext<OAuth2ServerOptions>
    {
        public TokenValidatedContext(HttpContext context,
            AuthenticationScheme scheme,
            OAuth2ServerOptions options)
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Get or set the token;
        /// </summary>
        public string Token { get; set; } = default!;
    }
}
