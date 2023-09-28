using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services
{
    public interface ITokenIntrospectionService
    {
        Task<TokenIntrospectionResponse> IntrospectTokenAsync(TokenIntrospectionRequest tokenIntrospectionRequest);
    }
}
