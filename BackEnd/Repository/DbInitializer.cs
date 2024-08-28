
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Repository;

public class DbInitializer
{
    public static async Task InitializeAsync(CapyLofiDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        context.Database.EnsureCreated();

        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            //Admin
            var roleAdmin = new IdentityRole<int> { Name = "Admin" };
            //Customers
            var roleCustomer = new IdentityRole<int> { Name = "Customer" };
            //Create Roles
            await roleManager.CreateAsync(roleAdmin);
            await roleManager.CreateAsync(roleCustomer);

        }

        // Seed Users
        await SeedUser(userManager, "goldvalory", "phuctran2003181@gmail.com", "Cubin2003@", "phuc", "goldvalory", 100, "dev manh nhat the gioi", "Admin");
        await SeedUser(userManager, "uyle123", "admin2@example.com", "Admin2#Password", "uy", "Admin2", 100, "Admin 2 profile", "Admin");

        // Seed Chat Data
        await SeedChatData(context);
    }

    private static async Task SeedUser(UserManager<User> userManager, string username, string email, string password, string name, string displayName, int coins, string profileInfo, string role)
    {
        if (await userManager.FindByNameAsync(username) == null)
        {
            var user = new User
            {
                UserName = username,
                Email = email,
                Name = name,
                DisplayName = displayName,
                Coins = coins,
                ProfileInfo = profileInfo,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                throw new Exception($"Failed to create user {username}: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    public static async Task SeedChatData(CapyLofiDbContext context)
    {
        // Seed Chat Rooms
        if (!context.ChatRooms.Any(cr => cr.Name == "Global"))
        {
            var chatRoom1 = new ChatRoom
            {
                Name = "Global",
                CountryCode = "GLOBAL",
                IsGlobal = true,
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };

            var chatRoom2 = new ChatRoom
            {
                Name = "Vietnam",
                CountryCode = "VN",
                IsGlobal = false,
                IsPrivate = false,
                CreatedAt = DateTime.Now
            };

            context.ChatRooms.Add(chatRoom1);
            context.ChatRooms.Add(chatRoom2);
            await context.SaveChangesAsync();
        }

        // Seed another chat room
        if (!context.ChatRooms.Any(cr => cr.Name == "Private Room"))
        {
            var privateChatRoom = new ChatRoom
            {
                Name = "Private Room",
                IsGlobal = false,
                IsPrivate = true,
                PrivateCode = "Private123",
                CreatedAt = DateTime.Now
            };

            context.ChatRooms.Add(privateChatRoom);
            await context.SaveChangesAsync();
        }

        // Seed Messages for Global Chat Room
        var globalChatRoom = context.ChatRooms.FirstOrDefault(cr => cr.Name == "Global");
        if (globalChatRoom != null && !context.Messages.Any(m => m.ChatRoomId == globalChatRoom.Id))
        {
            var messages = new List<Message>
            {
                new Message
                {
                    ChatRoomId = globalChatRoom.Id,
                    User = context.Users.FirstOrDefault(u => u.UserName == "goldvalory"),
                    Content = "Welcome to the Global Chat!",
                    CreatedAt = DateTime.Now
                },
                new Message
                {
                    ChatRoomId = globalChatRoom.Id,
                    User = context.Users.FirstOrDefault(u => u.UserName == "uyle123"),
                    Content = "Hello everyone!",
                    CreatedAt = DateTime.Now
                }
            };

            context.Messages.AddRange(messages);
            await context.SaveChangesAsync();
        }
    }
}


