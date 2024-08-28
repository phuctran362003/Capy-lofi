namespace Domain.Entities
{
    public class Message : BaseEntity
    {
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        // Navigation properties
        public ChatRoom ChatRoom { get; set; }
        public User User { get; set; }
    }
}
