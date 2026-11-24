using Microsoft.EntityFrameworkCore;
using OAuth20.Server.Models.Context;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth20.Server.Infrastructure;

public sealed class TokenRevocationStore : ITokenRevocationStore
{
    private readonly BaseDBContext _db;

    public TokenRevocationStore(BaseDBContext db)
    {
        _db = db;
    }

    public async Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default)
    {
        var token =
            await _db.OAuthTokens.FirstOrDefaultAsync(x => x.ReferenceId == jti, cancellationToken);

        if (token == null)
            return true;

        return token.Revoked;
    }
}
