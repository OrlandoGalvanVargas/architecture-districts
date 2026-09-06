using FacilityOS.API.Data;
using FacilityOS.Application.Common;
using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Settings;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Application.Features.Users.CreateUser;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace FacilityOS.API.Tests.Features.Users;

public class CreateUserHandlerTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static Mock<IResourceAuthorizationService> Authorization(bool canCreate = true, bool entityExists = true)
    {
        var auth = new Mock<IResourceAuthorizationService>();
        auth.Setup(x => x.CanCreateUserRoleAsync(It.IsAny<string>(), It.IsAny<UserEntityType>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(canCreate);
        auth.Setup(x => x.ValidateEntityExistsAsync(It.IsAny<UserEntityType>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entityExists);
        return auth;
    }

    private static IOptions<BCryptSettings> Settings() =>
        Options.Create(new BCryptSettings { WorkFactor = 4 });

    [Fact]
    public async Task Handle_ValidRequest_HashesPasswordAndCreatesUser()
    {
        await using var context = CreateContext();
        var handler = new CreateUserHandler(context, Authorization().Object, Settings());

        var result = await handler.Handle(new CreateUserCommand(new CreateUserRequest
        {
            Name = "District operator",
            Email = "operator@example.com",
            Password = "Secret123",
            Role = AppConstants.Roles.DistrictAdmin,
            EntityType = UserEntityType.District,
            EntityId = 2
        }), CancellationToken.None);

        var saved = await context.Users.SingleAsync();
        Assert.Equal("operator@example.com", result.Email);
        Assert.NotEqual("Secret123", saved.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("Secret123", saved.PasswordHash));
    }

    [Fact]
    public async Task Handle_InsufficientAuthorization_IsForbidden()
    {
        await using var context = CreateContext();
        var handler = new CreateUserHandler(context, Authorization(canCreate: false).Object, Settings());

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(
            new CreateUserCommand(new CreateUserRequest
            {
                Name = "User",
                Email = "user@example.com",
                Password = "Secret123",
                Role = AppConstants.Roles.DistrictAdmin
            }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MissingScopeEntity_IsNotFound()
    {
        await using var context = CreateContext();
        var handler = new CreateUserHandler(context, Authorization(entityExists: false).Object, Settings());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new CreateUserCommand(new CreateUserRequest
            {
                Name = "User",
                Email = "user@example.com",
                Password = "Secret123",
                Role = AppConstants.Roles.SchoolAdmin,
                EntityType = UserEntityType.School,
                EntityId = 7
            }), CancellationToken.None));
    }
}
