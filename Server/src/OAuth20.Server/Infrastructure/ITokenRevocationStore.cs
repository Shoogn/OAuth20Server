using System.Threading;
using System.Threading.Tasks;

namespace OAuth20.Server.Infrastructure;

// Keep OAuth concerns separate.
//The implementation can use OAuthTokens table to check if the token is revoked or not,
//but it should not be coupled with the implementation of the token store, so we can change the implementation of the token store without affecting the validation logic.
public interface ITokenRevocationStore
{
    Task<bool> IsRevokedAsync( string jti,  CancellationToken cancellationToken = default);
}
