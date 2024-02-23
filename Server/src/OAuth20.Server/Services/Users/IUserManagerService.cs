/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using OAuth20.Server.Models.Entities;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Users
{
    public interface IUserManagerService
    {
        Task<AppUser> GetUserAsync(string userId);
        Task<LoginResponse> LoginUserAsync(LoginRequest request);
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
        Task<OpenIdConnectLoginResponse> LoginUserByOpenIdAsync(OpenIdConnectLoginRequest request);
    }
}
