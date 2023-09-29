namespace ProtectedResourceApp_JwtBearer.Infrastructure
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
        /// Invoked when sending token introspection request.
        /// </summary>
        public virtual Task SendingTokenIntrospectionRequest(SendingTokenIntrospectionRequestContext context) 
            => OnSendingTokenIntrospectionRequest(context);

        /// <summary>
        /// Invoked after the token passed validation sucssefully.
        /// </summary>
        public virtual Task TokenValidated(TokenValidatedContext context) => OnTokenValidated(context);
    }
}
