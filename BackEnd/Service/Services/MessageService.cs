using Domain.Entities;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Message>> GetRecentMessagesByChatRoomIdAsync(int chatRoomId, int count = 10)
        {
            var messages = await _unitOfWork.MessageRepository.GetRecentMessagesByChatRoomIdAsync(chatRoomId, count);
            return messages.OrderByDescending(m => m.CreatedAt).Take(count).ToList();
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            await _unitOfWork.MessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangeAsync();
            return message;
        }
    }
}
