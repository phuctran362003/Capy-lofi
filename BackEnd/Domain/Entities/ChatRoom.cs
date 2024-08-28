namespace Domain.Entities
{
    public class ChatRoom : BaseEntity
    {
        public string? CountryCode { get; set; }
        public string? Name { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsPrivate { get; set; }
        public string? PrivateCode { get; set; }

        // Navigation properties
        public ICollection<Message> Messages { get; set; }
        public ICollection<UserChatRoom> UserChatRooms { get; set; }
        public ICollection<ChatInvitation> ChatInvitations { get; set; }
    }
}
