using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Grants
{
    public interface IGrantHandler
    {
        string GrantType { get; }
        Task<TokenResponse> HandleAsync(TokenRequest request);
    }
}
