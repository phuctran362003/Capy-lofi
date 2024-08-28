namespace Domain.DTOs.MessageDTOs
{
    public class MessageDTO
    {
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
