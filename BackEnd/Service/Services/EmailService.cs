using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class EmailService :  IEmailSender<User>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Phương thức duy nhất để gửi email
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(MailboxAddress.Parse(_configuration["EmailUserName"]));
        mimeMessage.To.Add(MailboxAddress.Parse(email));
        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlMessage
        };

        using var smtp = new SmtpClient();
        try
        {
            _logger.LogInformation("Connecting to email server...");
            await smtp.ConnectAsync(_configuration["EmailHost"], 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailUserName"], _configuration["EmailPassword"]);
            _logger.LogInformation("Sending email to {Email}", email);
            await smtp.SendAsync(mimeMessage);
            _logger.LogInformation("Email sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", email);
            // Handle the exception based on your application's requirements
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

    // Phương thức để gửi OTP
    public async Task SendOtpEmailAsync(string email, string otp)
    {
        var subject = "Email Verification OTP";
        var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #dddddd; border-radius: 10px;'>
                <h2 style='color: #333333;'>OTP Verification</h2>
                <p style='font-size: 16px; color: #555555;'>
                    Dear User,
                </p>
                <p style='font-size: 16px; color: #555555;'>
                    Your OTP (One-Time Password) for email verification is:
                </p>
                <h3 style='text-align: center; font-size: 24px; color: #2c7be5; margin: 20px 0;'>
                    {otp}
                </h3>
                <p style='font-size: 16px; color: #555555;'>
                    Please enter this code in the application to verify your email address. This code is valid for the next 10 minutes.
                </p>
                <p style='font-size: 16px; color: #555555;'>
                    If you did not request this OTP, please ignore this email or contact support if you have any concerns.
                </p>
                <br />
                <p style='font-size: 16px; color: #555555;'>
                    Thank you,
                    <br />
                    <strong>Your Company Name</strong>
                </p>
            </div>";

        await SendEmailAsync(email, subject, body);
    }

    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }
}
