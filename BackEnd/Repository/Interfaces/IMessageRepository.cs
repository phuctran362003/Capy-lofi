using Domain.Entities;

namespace Repository.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetRecentMessagesByChatRoomIdAsync(int chatRoomId, int count);
    }
}
