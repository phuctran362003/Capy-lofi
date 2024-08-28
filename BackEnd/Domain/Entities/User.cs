using Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
    // Additional properties that are not included in IdentityUser
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public int? Coins { get; set; }
    public string? ProfileInfo { get; set; }

    // The RefreshToken property can be handled differently if needed
    public string? RefreshToken { get; set; }
    public string? Otp { get; set; }
    public string? CountryCode { get; set; }

    // Navigation properties for related entities
    public ICollection<LearningSession> LearningSessions { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<UserMusic> UserMusics { get; set; }
    public ICollection<UserBackground> UserBackgrounds { get; set; }
    public ICollection<Feedback> Feedbacks { get; set; }
    public ICollection<UserChatRoom> UserChatRooms { get; set; }
    public ICollection<Message> Messages { get; set; }
}
