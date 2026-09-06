using FacilityOS.API.Data;
using FacilityOS.Application.Common;
using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.DTOs.Beacons;
using FacilityOS.Application.Features.Beacons.CreateBeacon;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using FacilityOS.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FacilityOS.API.Tests.Features.Beacons;

public class CreateBeaconHandlerTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static Mock<ICurrentUserService> AdminUser()
    {
        var user = new Mock<ICurrentUserService>();
        user.SetupGet(x => x.Role).Returns(AppConstants.Roles.Admin);
        user.SetupGet(x => x.IsAdmin).Returns(true);
        return user;
    }

    [Fact]
    public async Task Handle_Admin_NormalizesSerialAndCreatesAvailableBeacon()
    {
        await using var context = CreateContext();
        var handler = new CreateBeaconHandler(context, AdminUser().Object);

        var result = await handler.Handle(new CreateBeaconCommand(new CreateBeaconRequest
        {
            DeviceName = "Front entrance",
            SerialNumber = "  bcn-001 ",
            Type = BeaconType.Pendant
        }), CancellationToken.None);

        Assert.Equal("BCN-001", result.SerialNumber);
        Assert.Equal(BeaconStatus.Available, result.Status);
        Assert.Equal("BCN-001", (await context.Beacons.SingleAsync()).SerialNumber);
    }

    [Fact]
    public async Task Handle_NonAdmin_IsForbiddenBeforeWriting()
    {
        await using var context = CreateContext();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.IsAdmin).Returns(false);
        var handler = new CreateBeaconHandler(context, currentUser.Object);

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(
            new CreateBeaconCommand(new CreateBeaconRequest
            {
                DeviceName = "Beacon",
                SerialNumber = "BCN-002",
                Type = BeaconType.Fixed
            }), CancellationToken.None));

        Assert.Empty(context.Beacons);
    }

    [Fact]
    public async Task Handle_DuplicateSerial_IsConflict()
    {
        await using var context = CreateContext();
        context.Beacons.Add(new Beacon("Existing", "BCN-003", BeaconType.Mobile));
        await context.SaveChangesAsync();
        var handler = new CreateBeaconHandler(context, AdminUser().Object);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new CreateBeaconCommand(new CreateBeaconRequest
            {
                DeviceName = "Duplicate",
                SerialNumber = " bcn-003 ",
                Type = BeaconType.Mobile
            }), CancellationToken.None));
    }
}
