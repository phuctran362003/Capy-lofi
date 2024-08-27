using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Repository
{

    public class UserRepository : IUserRepository
    {
        //Microsoft.AspNetCore.Identity;
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateUserAsync(User user)
        {
            return await _userManager.CreateAsync(user);
        }

        public async Task UpdateOtpAsync(User user, string otpCode)
        {
            user.Otp = otpCode;
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> VerifyOtpAsync(User user, string otpCode)
        {
            return user.Otp == otpCode;
        }

        public async Task UpdateRefreshTokenAsync(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);
        }
    }


}