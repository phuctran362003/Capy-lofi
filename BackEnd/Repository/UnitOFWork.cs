using Repository.Interfaces;

namespace Repository
{
    public class UnitOFWork : IUnitOfWork
    {
        private readonly CapyLofiDbContext _context;
        private IUserRepository _userRepository;
        private IBackgroundRepository _backgroundRepository;
        private IMusicRepository _musicRepository;
        private IMessageRepository _messageRepository;
        private IChatRoomRepository _chatRoomRepository;
        private IChatInvitationRepository _chatInvitationRepository;

        public UnitOFWork(CapyLofiDbContext context, IUserRepository userRepository, IBackgroundRepository backgroundRepository, IMusicRepository musicRepository,
            IMessageRepository messageRepository, IChatRoomRepository chatRoomRepository, IChatInvitationRepository chatInvitationRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _backgroundRepository = backgroundRepository;
            _musicRepository = musicRepository;
            _messageRepository = messageRepository;
            _chatRoomRepository = chatRoomRepository;
            _chatInvitationRepository = chatInvitationRepository;
        }

        public IUserRepository UserRepository => _userRepository;
        public IBackgroundRepository BackgroundRepository => _backgroundRepository;
        public IMusicRepository MusicRepository => _musicRepository;
        public IMessageRepository MessageRepository => _messageRepository;
        public IChatRoomRepository ChatRoomRepository => _chatRoomRepository;
        public IChatInvitationRepository ChatInvitationRepository => _chatInvitationRepository;

        public Task<int> SaveChangeAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
