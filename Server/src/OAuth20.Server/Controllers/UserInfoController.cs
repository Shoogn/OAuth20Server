using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        [HttpGet, HttpPost]
        public async Task<IActionResult> UserInfo()
        {
            return Ok();
        }
    }
}
