namespace Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IBackgroundRepository BackgroundRepository { get; }
        IMusicRepository MusicRepository { get; }
        IMessageRepository MessageRepository { get; }
        IChatRoomRepository ChatRoomRepository { get; }
        IChatInvitationRepository ChatInvitationRepository { get; }
        Task<int> SaveChangeAsync();
    }
}
