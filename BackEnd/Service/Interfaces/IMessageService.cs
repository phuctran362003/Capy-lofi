using Domain.DTOs.MessageDTOs;
using Domain.Entities;

namespace Service.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageDTO>> GetRecentMessagesByChatRoomIdAsync(int chatRoomId, int count = 10);
        Task<MessageDTO> SendMessageAsync(Message message);
    }
}
