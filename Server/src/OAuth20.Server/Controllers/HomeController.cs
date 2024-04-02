/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OAuth20.Server.Models.Entities;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Services;
using OAuth20.Server.Services.CodeService;
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
        private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory;

        public HomeController(IHttpContextAccessor httpContextAccessor, IAuthorizeResultService authorizeResultService,
            ICodeStoreService codeStoreService, IUserManagerService userManagerService,
            IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizeResultService = authorizeResultService;
            _codeStoreService = codeStoreService;
            _userManagerService = userManagerService;
            _userClaimsPrincipalFactory= userClaimsPrincipalFactory;
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
                var updateCodeResult = _codeStoreService.UpdatedClientDataByCode(result.Code, 
                    _httpContextAccessor.HttpContext.User, result.RequestedScopes);
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
                RequestedScopes = result.RequestedScopes
                
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
                var claimsPrincipals = await _userClaimsPrincipalFactory.CreateAsync(userLoginResult.AppUser);
                var result = _codeStoreService.UpdatedClientDataByCode(loginRequest.Code,
                    claimsPrincipals, loginRequest.RequestedScopes);
                if (result != null)
                {
                    loginRequest.RedirectUri = loginRequest.RedirectUri + "&code=" + loginRequest.Code;
                    return Redirect(loginRequest.RedirectUri);
                }
            }
            return View("Error", new { error = "invalid_request" });
            //return RedirectToAction("Error", new { error = "invalid_request" });
        }

        [HttpPost]
        public JsonResult Token(TokenRequest tokenRequest)
        {
            var result = _authorizeResultService.GenerateToken(tokenRequest);

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
