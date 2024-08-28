using Domain.Entities;

namespace Repository.Interfaces
{
    public interface IChatRoomRepository : IGenericRepository<ChatRoom>
    {
        Task<ChatRoom> GetChatRoomByCountryCodeAsync(string countryCode);
        Task<IEnumerable<ChatRoom>> GetPublicChatRoomsAsync();
    }
}
