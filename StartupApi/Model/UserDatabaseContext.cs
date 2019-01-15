using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StartupApi.Model;

namespace StartupApi.Model
{
    public class UserDatabaseContext : IdentityDbContext<UserEntity, UserRoleEntity, Guid>
    {
        public UserDatabaseContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}