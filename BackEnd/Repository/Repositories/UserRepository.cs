using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Entities;

namespace Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {

        public UserRepository(CapyLofiDbContext context, ICurrentTime timeService, IClaimsService claimsService)
            : base(context, timeService, claimsService)
        {
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbSet.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbSet.FindAsync(userId);
        }

        public async Task CreateUserAsync(User user)
        {
            await _dbSet.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            _dbSet.Update(user);
        }
    }


}