using Domain.Entities;

namespace Service.Interfaces;

public interface IUserService
{
    Task<User> CreateOrUpdateUserAsync(string email, string name);
    Task<User> GetUserByIdAsync(int userId);
    Task UpdateUserAsync(User user);
}