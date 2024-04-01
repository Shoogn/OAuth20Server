/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

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
        public DbSet<DeviceFlowEntity> DeviceFlows { get; set; }
    }
}
