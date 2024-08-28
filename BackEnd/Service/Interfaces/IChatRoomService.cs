using Domain.Entities;

namespace Service.Interfaces
{
    public interface IChatRoomService
    {
        Task<ChatRoom> GetChatRoomByIdAsync(int chatRoomId);
        Task<ChatRoom> GetChatRoomByCountryCodeAsync(string countryCode);
        Task<IEnumerable<ChatRoom>> GetPublicChatRoomsAsync();
        Task<ChatRoom> CreateChatRoomAsync(ChatRoom chatRoom);
    }
}
