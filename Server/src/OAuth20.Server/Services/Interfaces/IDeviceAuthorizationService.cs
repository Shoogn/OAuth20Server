using Microsoft.AspNetCore.Http;
using OAuth20.Server.OAuthResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services
{
    public interface IDeviceAuthorizationService
    {
        Task<DeviceAuthorizationResponse> GenerateDeviceAuthorizationCodeAsync(HttpContext httpContext);
    }
}
