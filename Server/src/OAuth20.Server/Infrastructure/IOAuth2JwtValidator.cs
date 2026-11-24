using OAuth20.Server.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth20.Server.Infrastructure;

public interface IOAuth2JwtValidator
{
    Task<OAuth2JwtValidationResult> ValidateAsync(string token, OAuthServerOptions options, CancellationToken cancellationToken = default);
}
