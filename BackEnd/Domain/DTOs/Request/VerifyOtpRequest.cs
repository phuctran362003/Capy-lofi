﻿namespace Domain.DTOs.Request;

public class VerifyOtpRequest
{
    public string Email { get; set; }
    public string Otp { get; set; }
}