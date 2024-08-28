using Domain.Entities;

namespace Repository.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>> GetRecentMessagesByChatRoomIdAsync(int chatRoomId, int count);
    }
}
