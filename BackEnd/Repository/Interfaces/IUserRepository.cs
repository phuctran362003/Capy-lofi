using Domain.Entities;

namespace Repository.Interfaces;

public interface IUserRepository 
{

    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(int userId);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
}
