/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OAuth20.Server.Configuration;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Models.Entities;
using OAuth20.Server.Services;
using OAuth20.Server.Services.CodeServce;
using OAuth20.Server.Services.Users;
using OAuth20.Server.Validations;
using System;

var builder = WebApplication.CreateBuilder(args);
var configServices = builder.Configuration;
var connectionString = builder.Configuration.GetConnectionString("BaseDBConnection");
builder.Services.AddDbContext<BaseDBContext>(op =>
{
    op.UseSqlServer(connectionString);
});


builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<BaseDBContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Accounts/Login";
    options.AccessDeniedPath = "/Accounts/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
});


builder.Services.Configure<OAuthServerOptions>(configServices.GetSection("OAuthOptions"));
builder.Services.AddScoped<IAuthorizeResultService, AuthorizeResultService>();
builder.Services.AddSingleton<ICodeStoreService, CodeStoreService>();
builder.Services.AddScoped<IUserManagerService, UserManagerService>();
builder.Services.AddScoped<ITokenRevocationService, TokenRevocationService>();
builder.Services.AddScoped<ITokenIntrospectionService, TokenIntrospectionService>();
builder.Services.TryAddScoped<ITokenRevocationValidation, TokenRevocationValidation>();
builder.Services.TryAddScoped<ITokenIntrospectionValidation, TokenIntrospectionValidation>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});
builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run();
