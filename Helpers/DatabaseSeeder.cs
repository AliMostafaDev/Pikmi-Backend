using Microsoft.AspNetCore.Identity;
using Pikmi.API.Entities;

namespace Pikmi.API.Helpers
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            await SeedRolesAsync(roleManager);

            await SeedSuperAdminAsync(userManager, configuration);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "User", "Admin", "SuperAdmin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedSuperAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var superAdminEmail = configuration["SuperAdmin:Email"];
            var superAdminPassword = configuration["SuperAdmin:Password"];
            var superAdminFirstName = configuration["SuperAdmin:FirstName"];
            var superAdminLastName = configuration["SuperAdmin:LastName"];

            if (string.IsNullOrEmpty(superAdminEmail) || string.IsNullOrEmpty(superAdminPassword))
            {
                return;
            }

            var existingSuperAdmin = await userManager.FindByEmailAsync(superAdminEmail);
            if (existingSuperAdmin != null)
            {
                return;
            }

            var superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                FirstName = superAdminFirstName ?? "Super",
                LastName = superAdminLastName ?? "Admin",
                Gender = "Male",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsDocumentVerified = true,
                Balance = 10000, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(superAdmin, superAdminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                await userManager.AddToRoleAsync(superAdmin, "Admin");
            }

        }
    }
}
