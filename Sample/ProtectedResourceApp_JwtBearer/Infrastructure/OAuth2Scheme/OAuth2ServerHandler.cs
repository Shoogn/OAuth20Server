using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    // Made in love by Mohammed Ahmed Hussien
    public class OAuth2ServerHandler : AuthenticationHandler<OAuth2ServerOptions>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OAuth2ServerHandler(IOptionsMonitor<OAuth2ServerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpClientFactory httpClientFactory)
            : base(options, logger, encoder, clock)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected new OAuth2TokenIntrospectionEvent Events
        {
            get => (OAuth2TokenIntrospectionEvent)base.Events!;
            set => base.Events = value;
        }

        protected override Task<object> CreateEventsAsync()
        {
            return Task.FromResult<object>(new OAuth2TokenIntrospectionEvent());
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = string.Empty;
            try
            {
                string authorization = Request.Headers.Authorization.ToString();
                if (string.IsNullOrWhiteSpace(authorization))
                {
                    return AuthenticateResult.NoResult();
                }

                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }

                if (string.IsNullOrWhiteSpace(token))
                {
                    return AuthenticateResult.NoResult();
                }

                var requestSendingContext = new SendingTokenIntrospectionRequestContext(Context, Scheme, Options)
                {
                    Token = token,
                    TokenTypeHint = Options.TokenTypeHint
                };
                await Events.SendingTokenIntrospectionRequest(requestSendingContext);

                var client = _httpClientFactory.CreateClient(OAuth2IntrospectionJwtBearerDefaults.NamedBackChannelHttpClient);
                var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", token),
                    new KeyValuePair<string, string>("token_type_hint", Options.TokenTypeHint)
                };

                Uri baseUri = new Uri(Options.Authority);
                client.BaseAddress = baseUri;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;
                string credential = String.Format("{0}:{1}", Options.ClientId, Options.ClientSecret);
                string parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", parameters);
                using var req = new HttpRequestMessage(HttpMethod.Post, "/Introspections/TokenIntrospect") { Content = new FormUrlEncodedContent(values) };
                using var res = await client.SendAsync(req);

                if (res.IsSuccessStatusCode == false)
                {
                    return await AuthenticationFailedAsync(Context, Scheme, Options, Events,
                       $"Calling introspection endpoint is faild with this status code: {res.StatusCode}");
                }

                string responseBody = await res.Content.ReadAsStringAsync();
                TokenIntrospectionResponse? result = System.Text.Json.JsonSerializer.Deserialize<TokenIntrospectionResponse>(
                    responseBody, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Active ?? false)
                {
                    // Create ticket
                    var authenticationType = Options.AuthenticationType ?? Scheme.Name;
                    var claimIdentity = new ClaimsIdentity(result.Claims, authenticationType, "name", "role");
                    var claimPrinciple = new ClaimsPrincipal(claimIdentity);

                    TokenValidatedContext tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                    {
                        Principal = claimPrinciple,
                        Token = token
                    };
                    await Events.TokenValidated(tokenValidatedContext);

                    if (tokenValidatedContext.Result != null)
                    {
                        return tokenValidatedContext.Result;
                    }

                    if (Options.SaveToken)
                    {
                        tokenValidatedContext.Properties.StoreTokens(new[]
                        {
                            new AuthenticationToken { Name = "access_token", Value = token }
                        });
                    }
                    tokenValidatedContext.Success();
                    return tokenValidatedContext.Result!;
                }
                else
                {
                    return await AuthenticationFailedAsync(Context, Scheme, Options, Events, "The token is not active");
                }
            }
            catch (Exception ex)
            {
                return await AuthenticationFailedAsync(Context, Scheme, Options, Events, $"There is an exception {ex}");
            }

        }

        private static async Task<AuthenticateResult> AuthenticationFailedAsync(HttpContext context, 
            AuthenticationScheme authenticationScheme, 
            OAuth2ServerOptions options,
            OAuth2TokenIntrospectionEvent tokenIntrospectionEvent,
            string message)
        {
            AuthenticationFailedContext authenticationFailedContext = new AuthenticationFailedContext(context, authenticationScheme, options)
            {
                ErrorMessage = message
            };

            await tokenIntrospectionEvent.AuthenticationFailed(authenticationFailedContext);

            return authenticationFailedContext.Result != null
                        ? authenticationFailedContext.Result
                        : AuthenticateResult.Fail(message);
        }
        
    }
}
