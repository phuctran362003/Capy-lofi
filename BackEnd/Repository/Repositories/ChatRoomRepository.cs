using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class ChatRoomRepository : GenericRepository<ChatRoom>, IChatRoomRepository
    {
        private readonly CapyLofiDbContext _context;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;

        public ChatRoomRepository(CapyLofiDbContext context, ICurrentTime timeService, IClaimsService claimsService)
            : base(context, timeService, claimsService)
        {
            _context = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }

        // Additional methods specific to ChatRoom can be added here
        public async Task<ChatRoom> GetChatRoomByCountryCodeAsync(string countryCode)
        {
            return await _context.ChatRooms
                .FirstOrDefaultAsync(cr => cr.CountryCode.ToLower() == countryCode.ToLower());
        }

        public async Task<IEnumerable<ChatRoom>> GetPublicChatRoomsAsync()
        {
            return await _context.ChatRooms
                .Where(cr => !cr.IsPrivate)
                .ToListAsync();
        }
    }
}
