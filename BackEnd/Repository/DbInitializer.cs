using Domain.Entities;

namespace Repository;

public class DbInitializer
{
    public static void Initialize(CapyLofiDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Admins.Any())
        {
            return; 
        }

        var defaultAdmin = new Admin
        {
            Email = "admin@example.com", 
            Password = BCrypt.Net.BCrypt.HashPassword("Admin@123")
        };

        context.Admins.Add(defaultAdmin);
        context.SaveChanges();
    }
}

