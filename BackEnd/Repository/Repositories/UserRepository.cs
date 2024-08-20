using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Entities;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CapyLofiDbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(CapyLofiDbContext context, ICurrentTime timeService, IClaimsService claimsService)
        {
            _context = context;
            _dbSet = _context.Set<User>();
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
            await _context.SaveChangesAsync(); // Ensure changes are persisted to the database
        }

        public async Task UpdateUserAsync(User user)
        {
            _dbSet.Update(user);
            await _context.SaveChangesAsync(); // Ensure changes are persisted to the database
        }
    }



}