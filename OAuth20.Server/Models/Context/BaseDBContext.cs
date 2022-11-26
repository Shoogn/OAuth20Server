using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OAuth20.Server.Models.Entities;

namespace OAuth20.Server.Models.Context
{
    public class BaseDBContext : IdentityDbContext<AppUser>
    {
        public BaseDBContext(DbContextOptions<BaseDBContext> options) : base(options)
        {

        }
        public DbSet<OAuthApplicationEntity> OAuthApplications { get; set; }
        public DbSet<OAuthTokenEntity> OAuthTokens { get; set; }
    }
}
