using System;
using Microsoft.AspNetCore.Identity;

namespace StartupApi.Model
{
    public class UserRoleEntity : IdentityRole<Guid>
    {

        public UserRoleEntity() : base()
        {
        }

        public UserRoleEntity(string roleName) : base(roleName)
        {
        }
    }
}
