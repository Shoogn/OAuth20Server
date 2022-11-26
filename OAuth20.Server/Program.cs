using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth20.Server.Configuration;
using OAuth20.Server.Models.Context;
using OAuth20.Server.Models.Entities;
using OAuth20.Server.Services;
using OAuth20.Server.Services.CodeServce;

var builder = WebApplication.CreateBuilder(args);
var configServices = builder.Configuration;
var connectionString = builder.Configuration.GetConnectionString("BaseDBConnection");
builder.Services.AddDbContext<BaseDBContext>(op =>
{
    op.UseSqlServer(connectionString);
});

builder.Services.Configure<OAuthOptions>(configServices.GetSection("OAuthOptions"));
builder.Services.AddScoped<IAuthorizeResultService, AuthorizeResultService>();
builder.Services.AddSingleton<ICodeStoreService, CodeStoreService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 5;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<BaseDBContext>()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication();

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
