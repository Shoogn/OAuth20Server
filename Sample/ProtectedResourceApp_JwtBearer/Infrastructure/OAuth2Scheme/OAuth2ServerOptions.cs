using Microsoft.AspNetCore.Authentication;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    public class OAuth2ServerOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Get or set the OAuthrization URI.
        /// </summary>
        public string Authority { get; set; } = default!;

        /// <summary>
        /// Get or set the client Id.
        /// </summary>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Get or set the client secret.
        /// </summary>
        public string ClientSecret { get; set; } = default!;

        // <summary>
        //  Defines whether the bearer token should be stored in the
        //  Microsoft.AspNetCore.Authentication.AuthenticationProperties
        //  after a successful authorization.
        // </summary>
        public bool SaveToken { get; set; }

        /// <summary>
        ///  Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = "Bearer";

        /// <summary>
        /// The Backchannel used to retrieve metadata.
        /// </summary>
        public HttpClient BackChannel { get; set; } = default!;

        /// <summary>
        /// Get or set token type hint of the introspection client.
        /// </summary>
        public string TokenTypeHint { get; set; } = "access_token";

        /// <summary>
        /// Set the authentication type for the authenticated identity, null by default.
        /// </summary>
        public string? AuthenticationType { get; set; }
        /// <summary>
        /// Get or set the discovery endpoint to retrive all informations about OAuth2 server.
        /// </summary>
        //public string MetadataAddress { get; set; } = default!;

        /// <summary>
        /// Get or set the required scheme type for <see cref="MetadataAddress"/> 
        /// </summary>
        // public bool RequireHttpsMetadata { get; set; }

        public new OAuth2TokenIntrospectionEvent Events
        {
            get => (OAuth2TokenIntrospectionEvent)base.Events!;
            set => base.Events = value;
        }

        public override void Validate()
        {
            // Call the base validation to combine it with the custom validation. 
            base.Validate();

            if (string.IsNullOrWhiteSpace(Authority))
            {
                throw new ArgumentException(nameof(Authority));
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new ArgumentException(nameof(ClientId));
            }

            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new ArgumentException(nameof(ClientSecret));
            }
        }
    }
}
