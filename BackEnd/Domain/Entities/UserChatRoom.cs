namespace Domain.Entities
{
    public class UserChatRoom
    {
        public int UserId { get; set; }
        public int ChatRoomId { get; set; }
        public int? LastReadMessageId { get; set; }
        public bool IsOwner { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ChatRoom ChatRoom { get; set; }
        public Message LastReadMessage { get; set; }
    }
}
