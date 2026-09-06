using FacilityOS.API.Data;
using FacilityOS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FacilityOS.API.Tests.Features.Auth
{
    public class RefreshTokenHandlerTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static async Task<(ApplicationDbContext context, User user, RefreshToken token)>
            SeedUserWithRefreshToken(string tokenValue, bool isRevoked = false, DateTime? expiresAt = null)
        {
            var context = CreateInMemoryContext();

            var user = new User("Juan Perez", "juan@test.com", "hashed", "Admin");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var refreshToken = new RefreshToken(
                tokenValue,
                expiresAt ?? DateTime.UtcNow.AddDays(7),
                user.Id);
            if (isRevoked)
            {
                refreshToken.Revoke();
            }
            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();

            return (context, user, refreshToken);
        }

        [Fact]
        public async Task Handle_ValidToken_ReturnsNewTokensAndRevokesOld()
        {
            
            var (context, user, oldToken) = await SeedUserWithRefreshToken("valid-refresh-token");

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
                .Returns("new-access-token");
            mockAuthService
                .Setup(s => s.GenerateRefreshToken())
                .Returns("new-refresh-token");

            var handler = new RefreshTokenHandler(context, mockAuthService.Object, TestDoubles.JwtOptions());

            
            var result = await handler.Handle(
                new RefreshTokenCommand("valid-refresh-token"), CancellationToken.None);

            
            Assert.NotNull(result);
            Assert.Equal("new-access-token", result.AccessToken);
            Assert.Equal("new-refresh-token", result.RefreshToken);
            Assert.Equal(user.Email, result.User.Email);

            
            var revokedToken = await context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == "valid-refresh-token");
            Assert.NotNull(revokedToken);
            Assert.True(revokedToken.IsRevoked);

            
            var newToken = await context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == "new-refresh-token");
            Assert.NotNull(newToken);
            Assert.False(newToken.IsRevoked);
        }

        [Fact]
        public async Task Handle_RevokedToken_ThrowsUnauthorizedAccessException()
        {
            
            var (context, _, _) = await SeedUserWithRefreshToken(
                "revoked-token", isRevoked: true);

            var mockAuthService = new Mock<IAuthService>();
            var handler = new RefreshTokenHandler(context, mockAuthService.Object, TestDoubles.JwtOptions());

            
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new RefreshTokenCommand("revoked-token"), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ExpiredToken_ThrowsUnauthorizedAccessException()
        {
            
            var (context, _, _) = await SeedUserWithRefreshToken(
                "expired-token", expiresAt: DateTime.UtcNow.AddDays(-1)); 

            var mockAuthService = new Mock<IAuthService>();
            var handler = new RefreshTokenHandler(context, mockAuthService.Object, TestDoubles.JwtOptions());

            
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new RefreshTokenCommand("expired-token"), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TokenNotFound_ThrowsUnauthorizedAccessException()
        {
            
            using var context = CreateInMemoryContext();
            var mockAuthService = new Mock<IAuthService>();
            var handler = new RefreshTokenHandler(context, mockAuthService.Object, TestDoubles.JwtOptions());

            
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new RefreshTokenCommand("inexistente-token"), CancellationToken.None));
        }
    }
}
