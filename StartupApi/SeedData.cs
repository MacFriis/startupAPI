using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StartupApi.Model;

namespace StartupApi
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            await AddStandardUser(
                services.GetRequiredService<RoleManager<UserRoleEntity>>(),
                services.GetRequiredService<UserManager<UserEntity>>()
                );

            await AddTestData(services.GetRequiredService<AppDbContext>());
        }

        private static async Task AddTestData(AppDbContext context)
        {
            // TODO: Also check if the environment is test / dev /prod
            if (context.AppDatas.Any())
                return;

            await context.AddAsync(new AppDataEntity
            {
                Title = "Test",
                Subtitle = "Test data not valid for anything",
                Category = "Test"
            });

            await context.SaveChangesAsync();
        }

        private static async Task AddStandardUser(RoleManager<UserRoleEntity> roleManager, UserManager<UserEntity> userManager)
        {

            var dataExists = roleManager.Roles.Any() || userManager.Users.Any();
            if (dataExists)
                return;

            // Create the admin role and the root admin....
            await roleManager.CreateAsync(new UserRoleEntity("Admin"));

            var user = new UserEntity
            {
                Email = "ios@friisconsult.com",
                UserName = "ios@friisconsult.com",
                FirstName = "Per",
                LastName = "Friis",
                CreatedAt = DateTimeOffset.UtcNow
            };

            await userManager.CreateAsync(user, "Ub?r)um2%NPZMazakG9cwf8G");

            await userManager.AddToRoleAsync(user, "Admin");
            await userManager.UpdateAsync(user);

            // Create the root API user, current approach can be changed to use api key
            await roleManager.CreateAsync(new UserRoleEntity("api"));
            var apiuser = new UserEntity
            {
                Email = "api@friisconsult.com",
                UserName = "api@friisconsult.com",
                FirstName = "API",
                LastName = "Friis Mobility ApS",
                CreatedAt = DateTimeOffset.UtcNow
            };

            await userManager.CreateAsync(apiuser, "GurhRiRfuJCtf7mqwDqsr%FJdtdzrr,aJpLRc,pzAYnvQMUhWEofxE8zGpQUiYzT");
            await userManager.AddToRoleAsync(apiuser, "api");


            // TODO: create testuser if in test 

            if (true)
            {
                var testUser = new UserEntity
                {
                    Email = "test@friisconsult.com",
                    UserName = "test@friisconsult.com",
                    FirstName = "Teat",
                    LastName = "User",
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await userManager.CreateAsync(testUser, "Nescafe19?");
            }
        }
    }
}
