using FacilityOS.API.Data;
using FacilityOS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FacilityOS.API.Tests.Features.Auth
{
    public class LoginHandlerTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsLoginResponseWithTokens()
        {
            
            using var context = CreateInMemoryContext();

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("MySecurePass123", workFactor: 12);
            context.Users.Add(new User("Juan Perez", "juan@test.com", hashedPassword, "Admin"));
            await context.SaveChangesAsync();

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
                .Returns("fake-access-token");
            mockAuthService
                .Setup(s => s.GenerateRefreshToken())
                .Returns("fake-refresh-token");

            var handler = new LoginHandler(context, mockAuthService.Object, TestDoubles.JwtOptions());

            var request = new LoginRequest { Email = "juan@test.com", Password = "MySecurePass123" };

            
            var result = await handler.Handle(new LoginCommand(request), CancellationToken.None);

            
            Assert.NotNull(result);
            Assert.Equal("fake-access-token", result.AccessToken);
            Assert.Equal("fake-refresh-token", result.RefreshToken);
            Assert.Equal("juan@test.com", result.User.Email);

            var savedRefreshToken = await context.RefreshTokens.FirstOrDefaultAsync();
            Assert.NotNull(savedRefreshToken);
        }

        [Fact]
        public async Task Handle_WrongPassword_ThrowsUnauthorizedAccessException()
        {
            
            using var context = CreateInMemoryContext();

            context.Users.Add(new User(
                "Juan Perez",
                "juan@test.com",
                BCrypt.Net.BCrypt.HashPassword("CorrectPassword", workFactor: 12),
                "Admin"));
            await context.SaveChangesAsync();

            var mockAuthService = new Mock<IAuthService>();
            var handler = new LoginHandler(context, mockAuthService.Object, TestDoubles.JwtOptions());

            var request = new LoginRequest { Email = "juan@test.com", Password = "WrongPassword" };

            
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new LoginCommand(request), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmailDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            
            using var context = CreateInMemoryContext();
            var mockAuthService = new Mock<IAuthService>();
            var handler = new LoginHandler(context, mockAuthService.Object, TestDoubles.JwtOptions());

            var request = new LoginRequest { Email = "noexiste@test.com", Password = "AnyPassword123" };

            
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new LoginCommand(request), CancellationToken.None));
        }
    }
}
