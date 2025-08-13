using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using ECommerce.Business.Service;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
namespace ECommerce.Application.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserInRoleRepository> _mockUserInRoleRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserInRoleRepository = new Mock<IUserInRoleRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("super_secret_key_123!");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _mockConfiguration.Setup(c => c["Jwt:ExpiryInMinutes"]).Returns("60");

            _authService = new AuthService(
                _mockAuthRepository.Object,
                _mockConfiguration.Object,
                _mockUserRepository.Object,
                _mockUserInRoleRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenEmailNotExists()
        {
            // Arrange
            var registerDto = new registerDto("testuser", "test@example.com", "password");
            var userEntity = new User { UserId = 1, Email = registerDto.Email };

            _mockAuthRepository.Setup(r => r.UserExistsAsync(registerDto.Email))
                .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<User>(registerDto)).Returns(userEntity);
            _mockAuthRepository.Setup(r => r.RegisterAsync(userEntity))
                .ReturnsAsync(userEntity);
            _mockUserInRoleRepository.Setup(r => r.AddAsync(It.IsAny<UserInRole>()))
                .Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<UserDto>(userEntity))
                .Returns(new UserDto(1, "testuser", "test@example.com", "password"));

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(registerDto.Email, result.Email);
            _mockAuthRepository.Verify(r => r.RegisterAsync(userEntity), Times.Once);
            _mockUserInRoleRepository.Verify(r => r.AddAsync(It.IsAny<UserInRole>()), Times.Once);
        }
        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenEmailExists()
        {
            var registerDto = new registerDto("testuser", "test@example.com", "password");
            _mockAuthRepository.Setup(r => r.UserExistsAsync(registerDto.Email))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ApplicationException>(
                () => _authService.RegisterAsync(registerDto)
            );
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
        {
            var loginDto = new LoginDto("test@example.com", "password");

            var hashedPassword = _authService.GetType()
            .GetMethod("ComputeSha256Hash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(_authService, new object[] { loginDto.PasswordHash }).ToString();

            var user = new User { UserId = 1, Email = loginDto.Email, PasswordHash = hashedPassword };

            _mockAuthRepository.Setup(r => r.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _mockConfiguration.Setup(c => c["Jwt:Key"])
               .Returns("super_secret_key_1234567890123456");

            _mockUserInRoleRepository
          .Setup(r => r.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new UserInRole { UserId = 1, RoleId = 1 });

            // Act
            var token = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenInvalidCredentialssss()
        {
            var loginDto = new LoginDto("test@example.com", "wrongpass");

            var user = new User { UserId = 2, Email = loginDto.Email, PasswordHash = "17b2ba89601ba249fab3e1ce328756ee97fefdaaa4459db5c010953302fa4d28" };

            _mockAuthRepository.Setup(r => r.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockConfiguration.Setup(c => c["Jwt:Key"])
               .Returns("super_secret_key_1234567890123456");

            _mockUserInRoleRepository
          .Setup(r => r.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new UserInRole { UserId = 1, RoleId = 1 });

            string strtoken = "eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ0ZXN0QGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3NTUwOTE1MTcsImlzcyI6IlRlc3RJc3N1ZXIiLCJhdWQiOiJUZXN0QXVkaWVuY2UifQ";

            var token = await _authService.LoginAsync(loginDto);


            Assert.NotEqual(token, strtoken);


        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { UserId = 1, Email = "test@example.com", PasswordHash = "password" };
            _mockUserInRoleRepository
         .Setup(r => r.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
         .ReturnsAsync(new UserInRole { UserId = 1, RoleId = 1 });

            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto(1, "testuser", "test@example.com", "password"));

            _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            var result = await _authService.GetCurrentUserAsync(1);

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldThrow_WhenUserNotFound()
        {


            _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.GetCurrentUserAsync(1));
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldUpdatePassword_WhenCurrentPasswordCorrect()
        {
            var user = new User
            {
                UserId = 1,
                PasswordHash = _authService.GetType()
                .GetMethod("ComputeSha256Hash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_authService, new object[] { "oldpass" }).ToString()
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var dto = new UserDto(1, "testuser", "test@example.com", "oldpass");

            await _authService.ChangePasswordAsync(1, dto);

            _mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        public async Task ChangePasswordAsync_ShouldThrow_WhenUserNotFound()
        {
            _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            var dto = new UserDto(1, "testuser", "test@example.com", "pass");
            _mockUserInRoleRepository
       .Setup(r => r.GetByIdAsync(dto.UserId, It.IsAny<CancellationToken>()))
       .ReturnsAsync(new UserInRole { UserId = 1, RoleId = 1 });
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.ChangePasswordAsync(1, dto));
        }



    }
}
//using Xunit;
//using Moq;
//using ECommerce.Application.Features.Authentication.Commands;
//using ECommerce.Domain.Entities;
//using Microsoft.AspNetCore.Identity;

//public class RegisterUserCommandHandlerTests
//{
//    [Fact]
//    public async Task Handle_ValidCommand_ReturnsUserId()
//    {
//        // Arrange
//        var mockDbContext = new Mock<IApplicationDbContext>();
//        var mockPasswordHasher = new Mock<IPasswordHasher<User>>();

//        var users = new List<User>();
//        var mockDbSet = TestHelpers.MockDbSet(users);

//        mockDbContext.Setup(x => x.Users).Returns(mockDbSet.Object);
//        mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
//            .ReturnsAsync(1);

//        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
//            .Returns("hashed_password");

//        var handler = new RegisterUserCommandHandler(mockDbContext.Object, mockPasswordHasher.Object);
//        var command = new RegisterUserCommand("testuser", "test@example.com", "password");

//        // Act
//        var result = await handler.Handle(command, CancellationToken.None);

//        // Assert
//        Assert.NotEqual(Guid.Empty, result);
//        Assert.Single(users);
//        Assert.Equal("testuser", users[0].Username);
//        Assert.Equal("hashed_password", users[0].PasswordHash);
//    }
//}