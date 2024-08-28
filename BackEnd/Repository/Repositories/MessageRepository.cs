using Domain.Entities;
using Repository.Interfaces;
using System.Data.Entity;

namespace Repository.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly CapyLofiDbContext _context;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public MessageRepository(CapyLofiDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _context = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<IEnumerable<Message>> GetRecentMessagesByChatRoomIdAsync(int chatRoomId, int count)
        {
            return await _context.Messages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
