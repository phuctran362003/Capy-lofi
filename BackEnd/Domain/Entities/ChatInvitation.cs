namespace Domain.Entities
{
    public class ChatInvitation : BaseEntity
    {
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public string PrivateCode { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        public ChatRoom ChatRoom { get; set; }
        public User User { get; set; }
    }
}
