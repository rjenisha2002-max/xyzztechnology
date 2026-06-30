using AssetMgmt_WebApi.DTOs;

namespace AssetMgmt_WebApi.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> AuthenticateAsync(LoginDto dto);
    }
}
