using Microsoft.AspNetCore.Identity;
using Repository.Interfaces;

namespace Repository
{

    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }


        public async Task<User> GetUseByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }


        public async Task<IdentityResult> CreateDefaultUserAsync(User user)
        {
            // Kiểm tra và tạo vai trò "User" nếu chưa tồn tại
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var role = new IdentityRole<int> { Name = "User" };
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    return IdentityResult.Failed(roleResult.Errors.ToArray());
                }
            }

            var createResult = await _userManager.CreateAsync(user);

            if (createResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (!roleResult.Succeeded)
                {
                    var combinedErrors = createResult.Errors.Concat(roleResult.Errors);
                    return IdentityResult.Failed(combinedErrors.ToArray());
                }
            }
            return createResult;
        }

        public async Task<IdentityResult> CreateAdminUserAsync(User user)
        {
            // Kiểm tra và tạo vai trò "Admin" nếu chưa tồn tại
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole<int> { Name = "Admin" };
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    return IdentityResult.Failed(roleResult.Errors.ToArray());
                }
            }

            var createResult = await _userManager.CreateAsync(user);

            if (createResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");

                if (!roleResult.Succeeded)
                {
                    var combinedErrors = createResult.Errors.Concat(roleResult.Errors);
                    return IdentityResult.Failed(combinedErrors.ToArray());
                }
            }
            return createResult;
        }

        public async Task UpdateOtpAsync(User user, string hashedOtp)
        {
            user.Otp = hashedOtp;
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> VerifyOtpAsync(User user, string hashedOtp)
        {
            return user.Otp == hashedOtp;
        }

        public async Task UpdateRefreshTokenAsync(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateDisplayNameAsync(User user, string newDisplayName)
        {
            user.DisplayName = newDisplayName;
            var result = await _userManager.UpdateAsync(user);

            return result;
        }


    }



}