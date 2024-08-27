using Domain.DTOs;
using Domain.DTOs.Response;
using Domain.Entities;
using Newtonsoft.Json.Linq;
using Repository.Commons;

namespace Service.Interfaces;

public interface IAuthenService
{
    Task<ApiResult<string>> SendOtpAsync(string email);
    Task<ApiResult<Tokens>> VerifyOtpAsync(string email, string otpCode);
}
