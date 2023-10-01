/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Mvc;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Services.Users;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IUserManagerService _userManagerService;

        public AccountsController(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _userManagerService.LoginUserAsync(request);
            if(result.Succeeded)
                return RedirectToAction("Index", "Home");
            return View(request);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            var result = await _userManagerService.CreateUserAsync(request);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            return View(request);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
