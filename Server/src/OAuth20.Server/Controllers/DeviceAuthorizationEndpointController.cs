using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceAuthorizationEndpointController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeviceAuthorizationEndpointController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> Get()
        {
            return Ok();
        }
    }
}
