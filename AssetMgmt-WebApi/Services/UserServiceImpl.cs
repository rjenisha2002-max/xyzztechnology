using AssetMgmt_WebApi.DTOs;
using AssetMgmt_WebApi.Helpers;
using AssetMgmt_WebApi.Models;
using AssetMgmt_WebApi.Repositories;

namespace AssetMgmt_WebApi.Services
{
    //Business logic for user registration and login ---->model A
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly ILogger<UserServiceImpl> _logger;

        private static readonly string[] AllowedUserTypes = { "Admin", "Purchase Manager" };

        public UserServiceImpl(IUserRepository userRepository, JwtTokenHelper jwtTokenHelper,
            ILogger<UserServiceImpl> logger)
        {
            _userRepository = userRepository;
            _jwtTokenHelper = jwtTokenHelper;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            if (!AllowedUserTypes.Contains(dto.UserType))
            {
                return (false, "UserType must be either 'Admin' or 'Purchase Manager'.");
            }

            if (await _userRepository.UsernameExistsAsync(dto.Username))
            {
                _logger.LogWarning("Registration failed - username {Username} already exists", dto.Username);
                return (false, "Username already exists.");
            }

            var login = new Login
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                UserType = dto.UserType,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var registration = new UserRegistration
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                Gender = dto.Gender,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber
            };

            await _userRepository.AddUserAsync(login, registration);
            _logger.LogInformation("New user {Username} registered successfully", dto.Username);
            return (true, "User registered successfully.");
        }

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginDto dto)
        {
            var user = await _userRepository.GetLoginByUsernameAsync(dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                _logger.LogWarning("Authentication failed for user {Username}", dto.Username);
                return null;
            }

            var (token, expiresAt) = _jwtTokenHelper.GenerateToken(user);
            _logger.LogInformation("User {Username} authenticated successfully", dto.Username);

            return new LoginResponseDto
            {
                UserId = user.LId,
                Username = user.Username,
                UserType = user.UserType,
                FullName = user.UserRegistration != null
                    ? $"{user.UserRegistration.FirstName} {user.UserRegistration.LastName}"
                    : user.Username,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}
