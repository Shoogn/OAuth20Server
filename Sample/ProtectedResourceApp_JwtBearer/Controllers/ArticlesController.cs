using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProtectedResourceApp_JwtBearer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        [Authorize]
        [HttpGet("GetDummyData")]
        public IActionResult GetDummyData()
        {
            string result = "Calling to this endpoint at https://localhost:7065 is done successfully";
            return Ok(result);
        }
    }
}
