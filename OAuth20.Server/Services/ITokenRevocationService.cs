using Microsoft.AspNetCore.Http;
using OAuth20.Server.OauthResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services
{
    public interface ITokenRevocationService
    {
        Task<TokenRecovationResponse> RevokeTokenAsync(HttpContext httpContext, string clientId);
    }
}
