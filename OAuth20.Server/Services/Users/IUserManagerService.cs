using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Users
{
    public interface IUserManagerService
    {
        Task<LoginResponse> LoginUserAsync(LoginRequest request);
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
    }
}
