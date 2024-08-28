using Domain.Entities;

namespace Service.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetRecentMessagesByChatRoomIdAsync(int chatRoomId, int count = 10);
        Task<Message> SendMessageAsync(Message message);
    }
}
