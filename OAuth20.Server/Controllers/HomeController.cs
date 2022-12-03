using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Services;
using OAuth20.Server.Services.CodeServce;
using OAuth20.Server.Services.Users;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    public class HomeController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizeResultService _authorizeResultService;
        private readonly ICodeStoreService _codeStoreService;
        private readonly IUserManagerService _userManagerService;
        public HomeController(IHttpContextAccessor httpContextAccessor, IAuthorizeResultService authorizeResultService,
            ICodeStoreService codeStoreService, IUserManagerService userManagerService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizeResultService = authorizeResultService;
            _codeStoreService = codeStoreService;
            _userManagerService = userManagerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Authorize(AuthorizationRequest authorizationRequest)
        {
            var result = _authorizeResultService.AuthorizeRequest(_httpContextAccessor, authorizationRequest);

            if (result.HasError)
                return RedirectToAction("Error", new { error = result.Error });

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var updateCodeResult = _codeStoreService.UpdatedClientDataByCode(result.Code, result.RequestedScopes);
                if (updateCodeResult != null)
                {
                    result.RedirectUri = result.RedirectUri + "&code=" + result.Code;
                    return Redirect(result.RedirectUri);
                }
                else
                {
                    return RedirectToAction("Error", new { error = "invalid_request" });
                }

            }

            var loginModel = new OpenIdConnectLoginRequest
            {
                RedirectUri = result.RedirectUri,
                Code = result.Code,
                RequestedScopes = result.RequestedScopes,
            };
            return View("Login", loginModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(OpenIdConnectLoginRequest loginRequest)
        {
            // here I have to check if the username and passowrd is correct
            // and I will show you how to integrate the ASP.NET Core Identity
            // With our framework

            if (!loginRequest.IsValid())
                return RedirectToAction("Error", new { error = "invalid_request" });
            var userLoginResult = await _userManagerService.LoginUserByOpenIdAsync(loginRequest);

            if (userLoginResult.Succeeded)
            {
                var result = _codeStoreService.UpdatedClientDataByCode(loginRequest.Code, loginRequest.RequestedScopes);
                if (result != null)
                {
                    loginRequest.RedirectUri = loginRequest.RedirectUri + "&code=" + loginRequest.Code;
                    return Redirect(loginRequest.RedirectUri);
                }
            }

            return RedirectToAction("Error", new { error = "invalid_request" });
        }

        public JsonResult Token()
        {
            var result = _authorizeResultService.GenerateToken(_httpContextAccessor);

            if (result.HasError)
                return Json(new
                {
                    error = result.Error,
                    error_description = result.ErrorDescription
                });

            return Json(result);
        }
        public IActionResult Error(string error)
        {
            return View(error);
        }
    }
}
