using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OAuth20.Server.Services;
using OAuth20.Server.Services.CodeServce;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthorizeResultService, AuthorizeResultService>();
builder.Services.AddSingleton<ICodeStoreService, CodeStoreService>();
builder.Services.AddHttpContextAccessor();
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
    //endpoints.MapControllerRoute(".well-known/openid-configuration", "{controller}/{action}/{id?}",
    //    defaults: new { controller = "DiscoveryEndpoint", action = "GetConfiguration" });
});

//app.MapDefaultControllerRoute();
app.Run();
