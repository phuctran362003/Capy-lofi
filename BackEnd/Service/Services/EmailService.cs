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
using Domain.DTOs.EmailDTOs;
using Service.Interfaces;


//tải package EmailSender của Identity nha
public class EmailService : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

        using var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(_configuration["EmailHost"], 587, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_configuration["EmailUserName"], _configuration["EmailPassword"]);
            await smtpClient.SendAsync(mimeMessage);
            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", email);
            throw;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
    }

    public async Task SendOtpAsync(string email, string otpCode)
    {
        var emailDto = new EmailDTO
        {
            To = email,
            Subject = "Your OTP Code",
            Body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #68D391;'> <!-- bg-green-400 -->
                    <h2 style='color: #333;'>Your OTP Code</h2>
                    <p style='color: #555;'>Here is your One-Time Password (OTP) to access your account:</p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <span style='font-size: 24px; font-weight: bold; color: #333;'>{otpCode}</span>
                    </div>
                    <p style='color: #555;'>Please use this code to complete your login. This code is valid for 10 minutes.</p>
                    <p style='color: #555;'>If you did not request this code, please ignore this email.</p>
                    <p style='color: #555;'>Best regards,<br />Your Team</p>
                </div>
            </body>
            </html>"
        };

        await SendEmailAsync(email, emailDto.Subject, emailDto.Body);
    }

}


