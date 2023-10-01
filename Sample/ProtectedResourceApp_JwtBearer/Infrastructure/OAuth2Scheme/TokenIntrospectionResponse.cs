using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme
{
    public class TokenIntrospectionResponse
    {
        // <summary>
        /// Is Token Active / Required parameter
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Associated scopes with this token,
        /// Return as space-sperated list
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// Client Identifier for OAuth client that 
        /// requested this token
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Resource owner who authorized this token
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Access Token or Refresh Token
        /// </summary>
        public string? TokenType { get; set; }

        /// <summary>
        /// Expiration Time
        /// </summary>
        public int Exp { get; set; }

        /// <summary>
        /// Issued At
        /// </summary>
        public int Iat { get; set; }

        /// <summary>
        /// Not Before
        /// </summary>
        public int Nbf { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string? Sub { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string? Aud { get; set; }

        /// <summary>
        /// Issuer
        /// </summary>
        public string? Iss { get; set; }
        /// <summary>
        /// JWT ID
        /// </summary>
        public string? Jti { get; set; }

        /// <summary>
        /// Tranfere scope that are comming after calling the introspection endpoint to Claim object.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.Always)]
        public IEnumerable<Claim> Claims
        {
            get
            {
                var claims = new List<Claim>();
                var s = Scope?.Split(' ');
                foreach (var item in s ?? new string[] { })
                {
                    var claim = item.Trim();
                    claims.Add(new Claim("scope", claim));
                }
                return claims;
            }
        }
    }
}
