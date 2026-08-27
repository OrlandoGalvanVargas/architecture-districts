using FacilityOS.API.Data;
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

        private static async Task<(ApplicationDbContext context, User user, FacilityOS.API.Models.RefreshToken token)>
            SeedUserWithRefreshToken(string tokenValue, bool isRevoked = false, DateTime? expiresAt = null)
        {
            var context = CreateInMemoryContext();

            var user = new User
            {
                Name = "Juan Perez",
                Email = "juan@test.com",
                PasswordHash = "hashed",
                Role = "Admin"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var refreshToken = new FacilityOS.API.Models.RefreshToken
            {
                Token = tokenValue,
                UserId = user.Id,
                IsRevoked = isRevoked,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();

            return (context, user, refreshToken);
        }

        [Fact]
        public async Task Handle_ValidToken_ReturnsNewTokensAndRevokesOld()
        {
            // Arrange
            var (context, user, oldToken) = await SeedUserWithRefreshToken("valid-refresh-token");

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
                .Returns("new-access-token");
            mockAuthService
                .Setup(s => s.GenerateRefreshToken())
                .Returns("new-refresh-token");

            var handler = new RefreshTokenHandler(context, mockAuthService.Object);

            // Act
            var result = await handler.Handle(
                new RefreshTokenCommand("valid-refresh-token"), CancellationToken.None);

            // Assert — verifica la respuesta
            Assert.NotNull(result);
            Assert.Equal("new-access-token", result.AccessToken);
            Assert.Equal("new-refresh-token", result.RefreshToken);
            Assert.Equal(user.Email, result.User.Email);

            // Assert — verifica la rotación: token viejo revocado
            var revokedToken = await context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == "valid-refresh-token");
            Assert.NotNull(revokedToken);
            Assert.True(revokedToken.IsRevoked);

            // Assert — verifica que el token nuevo existe en BD
            var newToken = await context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == "new-refresh-token");
            Assert.NotNull(newToken);
            Assert.False(newToken.IsRevoked);
        }

        [Fact]
        public async Task Handle_RevokedToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var (context, _, _) = await SeedUserWithRefreshToken(
                "revoked-token", isRevoked: true);

            var mockAuthService = new Mock<IAuthService>();
            var handler = new RefreshTokenHandler(context, mockAuthService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new RefreshTokenCommand("revoked-token"), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ExpiredToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var (context, _, _) = await SeedUserWithRefreshToken(
                "expired-token", expiresAt: DateTime.UtcNow.AddDays(-1)); // venció ayer

            var mockAuthService = new Mock<IAuthService>();
            var handler = new RefreshTokenHandler(context, mockAuthService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new RefreshTokenCommand("expired-token"), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TokenNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockAuthService = new Mock<IAuthService>();
            var handler = new RefreshTokenHandler(context, mockAuthService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(new RefreshTokenCommand("inexistente-token"), CancellationToken.None));
        }
    }
}
