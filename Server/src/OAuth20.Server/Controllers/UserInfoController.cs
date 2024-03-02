using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAuth20.Server.Services;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("UserInfoPolicy")]
    [ApiController]
    [AllowAnonymous]
    public class UserInfoController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        [HttpGet("GetUserInfo")]

        public async Task<IActionResult> GetUserInfo()
        {
            var userInfo = await _userInfoService.GetUserInfoAsync();
            return Ok(userInfo);
        }
    }
}
