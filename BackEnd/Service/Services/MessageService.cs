using AutoMapper;
using Domain.DTOs.MessageDTOs;
using Domain.Entities;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MessageDTO>> GetRecentMessagesByChatRoomIdAsync(int chatRoomId, int count = 10)
        {
            var messages = await _unitOfWork.MessageRepository.GetRecentMessagesByChatRoomIdAsync(chatRoomId, count);
            return _mapper.Map<List<MessageDTO>>(messages);
        }

        public async Task<MessageDTO> SendMessageAsync(Message message)
        {
            var newMessage = await _unitOfWork.MessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangeAsync();
            return _mapper.Map<MessageDTO>(newMessage);
        }
    }
}
