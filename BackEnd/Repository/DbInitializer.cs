
using Microsoft.AspNetCore.Identity;

namespace Repository;

public class DbInitializer
{
    public static async Task InitializeAsync(CapyLofiDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        context.Database.EnsureCreated();
        
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var role = new IdentityRole<int> { Name = "Admin" };
            await roleManager.CreateAsync(role);
        }

        if (await userManager.FindByNameAsync("goldvalory") == null)
        {
            var admin1 = new User
            {
                UserName = "goldvalory",
                Email = "phuctran2003181@gmail.com",
                Name = "phuc",
                DisplayName = "goldvalory",
                Coins = 100,
                ProfileInfo = "dev manh nhat the gioi",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result1 = await userManager.CreateAsync(admin1, "Cubin2003@");
            if (result1.Succeeded)
            {
                await userManager.AddToRoleAsync(admin1, "Admin");
            }
            else
            {
                throw new Exception("Failed to create admin1: " + string.Join(", ", result1.Errors.Select(e => e.Description)));
            }
        }

        if (await userManager.FindByNameAsync("uyle123") == null)
        {
            var admin2 = new User
            {
                UserName = "uyle123",
                Email = "admin2@example.com",
                Name = "uy",
                DisplayName = "Admin2",
                Coins = 100,
                ProfileInfo = "Admin 2 profile",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result2 = await userManager.CreateAsync(admin2, "Admin2#Password");
            if (result2.Succeeded)
            {
                await userManager.AddToRoleAsync(admin2, "Admin");
            }
            else
            {
                throw new Exception("Failed to create admin2: " + string.Join(", ", result2.Errors.Select(e => e.Description)));
            }
        }
    }
}


