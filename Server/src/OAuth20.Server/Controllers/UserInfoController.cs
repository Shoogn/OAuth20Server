using Microsoft.AspNetCore.Mvc;
using OAuth20.Server.Services;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }
        [HttpGet, HttpPost]
        public async Task<IActionResult> GetUserInfo()
        {
            var userInfo = await _userInfoService.GetUserInfoAsync();
            return Ok(userInfo);
        }
    }
}
