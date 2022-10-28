using Microsoft.AspNetCore.Http;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;

namespace OAuth20.Server.Services
{
    public interface IAuthorizeResultService
    {
        AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest);
        TokenResponse GenerateToken(IHttpContextAccessor httpContextAccessor);
    }
}
