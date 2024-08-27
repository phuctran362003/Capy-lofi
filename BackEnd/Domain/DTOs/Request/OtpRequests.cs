using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Request;
public class OtpRequests
{
    public class SendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }

    }
    public class VerifyOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }
    }
}
