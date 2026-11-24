using System.Collections.Generic;

namespace OAuth20.Server.Infrastructure;

public class OAuth2JwtPayload
{
    public string Sub { get; set; }
    public string Iss { get; set; }
    public string Aud { get; set; }
    /// <summary>
    /// Get or set the type of the token from generating the token request from the type: ""JWT" or "at+jwt" or "refresh_token".
    /// </summary>
    public string TokenType { get; set; }

    public long Exp { get; set; }
    public long Nbf { get; set; }
    public long Iat { get; set; }
    public string Jti { get; set; }
    public Dictionary<string, object> Claims { get; set; } = new Dictionary<string, object>();
}
