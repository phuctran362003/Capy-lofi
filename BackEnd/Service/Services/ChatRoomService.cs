using Domain.Entities;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChatRoom> GetChatRoomByIdAsync(int chatRoomId)
        {
            return await _unitOfWork.ChatRoomRepository.GetByIdAsync(chatRoomId);
        }

        public async Task<ChatRoom> GetChatRoomByCountryCodeAsync(string countryCode)
        {
            return await _unitOfWork.ChatRoomRepository.GetChatRoomByCountryCodeAsync(countryCode);
        }

        public async Task<IEnumerable<ChatRoom>> GetPublicChatRoomsAsync()
        {
            return await _unitOfWork.ChatRoomRepository.GetPublicChatRoomsAsync();
        }

        public async Task<ChatRoom> CreateChatRoomAsync(ChatRoom chatRoom)
        {
            await _unitOfWork.ChatRoomRepository.AddAsync(chatRoom);
            await _unitOfWork.SaveChangeAsync();
            return chatRoom;
        }
    }
}
