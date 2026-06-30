using AssetMgmt_WebApi.DTOs;
using AssetMgmt_WebApi.Helpers;
using AssetMgmt_WebApi.Models;
using AssetMgmt_WebApi.Repositories;
using AssetMgmt_WebApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace AssetMgmt_WebApi.Tests
{
    
    public class UserServiceTests
    {
        private static JwtTokenHelper BuildJwtHelper()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "UnitTest_SecretKey_For_AssetMgmt_TestSuite_2026_LongEnough" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpiryMinutes", "60" }
            };
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            return new JwtTokenHelper(config);
        }

        [Fact]
        public async Task RegisterAsync_RejectsDuplicateUsername()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.UsernameExistsAsync("jdoe")).ReturnsAsync(true);

            var service = new UserServiceImpl(repo.Object, BuildJwtHelper(), NullLogger<UserServiceImpl>.Instance);

            var dto = new RegisterDto
            {
                Username = "jdoe",
                Password = "Pass@123",
                UserType = "Admin",
                FirstName = "John",
                LastName = "Doe",
                Age = 28,
                Gender = "Male",
                Address = "Kochi",
                PhoneNumber = "9876543210"
            };

            var (success, message) = await service.RegisterAsync(dto);

            Assert.False(success);
            Assert.Equal("Username already exists.", message);
            repo.Verify(r => r.AddUserAsync(It.IsAny<Login>(), It.IsAny<UserRegistration>()), Times.Never);
        }

        [Theory]
        [InlineData("SuperAdmin")]
        [InlineData("")]
        public async Task RegisterAsync_RejectsInvalidUserType(string userType)
        {
            var repo = new Mock<IUserRepository>();
            var service = new UserServiceImpl(repo.Object, BuildJwtHelper(), NullLogger<UserServiceImpl>.Instance);

            var dto = new RegisterDto
            {
                Username = "newuser",
                Password = "Pass@123",
                UserType = userType,
                FirstName = "Jane",
                LastName = "Roe",
                Age = 25,
                Gender = "Female",
                Address = "Kochi",
                PhoneNumber = "9876543211"
            };

            var (success, message) = await service.RegisterAsync(dto);

            Assert.False(success);
            Assert.Contains("UserType", message);
        }

        [Fact]
        public async Task RegisterAsync_SucceedsForValidNewUser()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.UsernameExistsAsync("newuser")).ReturnsAsync(false);
            repo.Setup(r => r.AddUserAsync(It.IsAny<Login>(), It.IsAny<UserRegistration>()))
                .ReturnsAsync((Login l, UserRegistration u) => l);

            var service = new UserServiceImpl(repo.Object, BuildJwtHelper(), NullLogger<UserServiceImpl>.Instance);

            var dto = new RegisterDto
            {
                Username = "newuser",
                Password = "Pass@123",
                UserType = "Purchase Manager",
                FirstName = "Jane",
                LastName = "Roe",
                Age = 25,
                Gender = "Female",
                Address = "Kochi",
                PhoneNumber = "9876543211"
            };

            var (success, message) = await service.RegisterAsync(dto);

            Assert.True(success);
            repo.Verify(r => r.AddUserAsync(It.IsAny<Login>(), It.IsAny<UserRegistration>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsNull_ForUnknownUsername()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetLoginByUsernameAsync("ghost")).ReturnsAsync((Login?)null);

            var service = new UserServiceImpl(repo.Object, BuildJwtHelper(), NullLogger<UserServiceImpl>.Instance);

            var result = await service.AuthenticateAsync(new LoginDto { Username = "ghost", Password = "whatever" });

            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsNull_ForWrongPassword()
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword("CorrectPass1");
            var existingUser = new Login { LId = 1, Username = "jdoe", Password = hashed, UserType = "Admin", IsActive = true };

            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetLoginByUsernameAsync("jdoe")).ReturnsAsync(existingUser);

            var service = new UserServiceImpl(repo.Object, BuildJwtHelper(), NullLogger<UserServiceImpl>.Instance);

            var result = await service.AuthenticateAsync(new LoginDto { Username = "jdoe", Password = "WrongPass" });

            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsToken_ForCorrectCredentials()
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword("CorrectPass1");
            var existingUser = new Login
            {
                LId = 1,
                Username = "jdoe",
                Password = hashed,
                UserType = "Admin",
                IsActive = true,
                UserRegistration = new UserRegistration { FirstName = "John", LastName = "Doe" }
            };

            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetLoginByUsernameAsync("jdoe")).ReturnsAsync(existingUser);

            var service = new UserServiceImpl(repo.Object, BuildJwtHelper(), NullLogger<UserServiceImpl>.Instance);

            var result = await service.AuthenticateAsync(new LoginDto { Username = "jdoe", Password = "CorrectPass1" });

            Assert.NotNull(result);
            Assert.Equal("jdoe", result!.Username);
            Assert.Equal("Admin", result.UserType);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }
    }
}
