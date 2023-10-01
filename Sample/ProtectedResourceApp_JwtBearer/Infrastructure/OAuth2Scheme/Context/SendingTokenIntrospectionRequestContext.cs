using Microsoft.AspNetCore.Authentication;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    public class SendingTokenIntrospectionRequestContext : BaseContext<OAuth2ServerOptions>
    {
        public SendingTokenIntrospectionRequestContext(HttpContext context, 
            AuthenticationScheme scheme, 
            OAuth2ServerOptions options) 
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Get or set token;
        /// </summary>
        public string Token { get; set; } = default!;

        /// <summary>
        /// Get or set token type hint.
        /// </summary>
        public string TokenTypeHint { get; set; } = default!;
    }
}
