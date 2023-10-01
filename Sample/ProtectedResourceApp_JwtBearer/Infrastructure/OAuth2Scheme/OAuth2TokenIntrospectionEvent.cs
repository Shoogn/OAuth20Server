using ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    /// <summary>
    ///  Specifies events which the <see cref="OAuth2ServerHandler"/> invokes to enable developer control over the authentication process.
    /// </summary>
    public class OAuth2TokenIntrospectionEvent
    {
        /// <summary>
        /// Invoked after the token passed validation sucssefully.
        /// </summary>
        public Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked when sending token introspection request.
        /// </summary>
        public Func<SendingTokenIntrospectionRequestContext, Task> OnSendingTokenIntrospectionRequest { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked when the authentication failed.
        /// </summary>
        public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked when sending token introspection request.
        /// </summary>
        public virtual Task SendingTokenIntrospectionRequest(SendingTokenIntrospectionRequestContext context) 
            => OnSendingTokenIntrospectionRequest(context);

        /// <summary>
        /// Invoked after the token passed validation sucssefully.
        /// </summary>
        public virtual Task TokenValidated(TokenValidatedContext context) => OnTokenValidated(context);

        /// <summary>
        /// Invoked when the authentication failed.
        /// </summary>
        public virtual Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);
    }
}
