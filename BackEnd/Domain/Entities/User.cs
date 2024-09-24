using Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
    // Additional properties
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public int? Coins { get; set; }
    public string? ProfileInfo { get; set; }
    public string? RefreshToken { get; set; }
    public string? Otp { get; set; }
    public string? OtpExpiryTime { get; set; }
    public string? CountryCode { get; set; }

    // Navigation properties
    public ICollection<LearningSession> LearningSessions { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<UserMusic> UserMusics { get; set; }
    public ICollection<UserBackground> UserBackgrounds { get; set; }
    public ICollection<Feedback> Feedbacks { get; set; }
    public ICollection<UserChatRoom> UserChatRooms { get; set; }
    public ICollection<Message> Messages { get; set; }

    public User()
    {
        Coins = 0;
        ProfileInfo = "Welcome to the platform!";
        LearningSessions = new List<LearningSession>();
        Orders = new List<Order>();
        UserMusics = new List<UserMusic>();
        UserBackgrounds = new List<UserBackground>();
        Feedbacks = new List<Feedback>();
        UserChatRooms = new List<UserChatRoom>();
        Messages = new List<Message>();
    }
}

