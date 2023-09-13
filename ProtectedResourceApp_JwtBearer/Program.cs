var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:7275"; // This is the OAuth20.Server URI
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("jwtapitestapp", policy =>
    {
        //policy.RequireAuthenticatedUser();
        policy.RequireClaim("scopes", "jwtapitestapp.read");
    });
});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization("jwtapitestapp");

app.Run();
