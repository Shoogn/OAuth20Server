using Microsoft.AspNetCore.Http;
using OAuth20.Server.Validations.Response;
using System.Threading.Tasks;

namespace OAuth20.Server.Validations
{
    public interface IDeviceAuthorizationValidation
    {
        Task<DeviceAuthorizationValidationResponse> ValidateAsync(HttpContext httpContext);
    }
}
