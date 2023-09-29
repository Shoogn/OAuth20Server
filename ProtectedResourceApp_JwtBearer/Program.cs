using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProtectedResourceApp_JwtBearer.Infrastructure;
using ProtectedResourceApp_JwtBearer.Infrastructure.OAuth2Scheme;

var builder = WebApplication.CreateBuilder(args);

//"Bearer"
builder.Services.AddControllers();
builder.Services.AddAuthentication(OAuth2IntrospectionJwtBearerDefaults.AuthenticationScheme)
    .AddOAuth2IntrospectionJwtBearer(OAuth2IntrospectionJwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "https://localhost:7275";
        options.ClientId = "3";
        options.ClientSecret = "123456789";
        options.SaveToken = true;
    }) ;
    //.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    //{
    //    options.Authority = "https://localhost:7275"; // This is the OAuth20.Server URI
    //    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    //    {
    //        ValidateAudience = false,
    //    };
    //});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("jwtapitestapp", policy =>
    {
        //policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "jwtapitestapp.read");
    });
});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization("jwtapitestapp");

app.Run();
