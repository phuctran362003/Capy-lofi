using Domain.Entities;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> CreateOrUpdateUserAsync(string email, string name)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        
        if (existingUser != null)
        {
            existingUser.Name = name;
            await _userRepository.UpdateUserAsync(existingUser);
            return existingUser;
        }
        else
        {
            var newUser = new User
            {
                Email = email,
                Name = name,
                CreatedAt = DateTime.UtcNow 
            };
            
            await _userRepository.CreateUserAsync(newUser);
            return newUser;
        }
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateUserAsync(user);
    }
}


