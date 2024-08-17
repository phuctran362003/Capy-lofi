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
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            user = new User { Email = email, Name = name };
            await _userRepository.CreateUserAsync(user);
        }
        else
        {
            user.Name = name;
            await _userRepository.UpdateUserAsync(user);
        }
        return user;
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

