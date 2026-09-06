using FacilityOS.Application.Common;
using FacilityOS.Application.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace FacilityOS.API.Tests;

internal static class TestDoubles
{
    public static ICurrentUserService AdminUser()
    {
        var user = new Mock<ICurrentUserService>();
        user.SetupGet(x => x.IsAdmin).Returns(true);
        user.SetupGet(x => x.Role).Returns(AppConstants.Roles.Admin);
        return user.Object;
    }

    public static IResourceAuthorizationService AllowAllResources()
    {
        var auth = new Mock<IResourceAuthorizationService>();
        auth.Setup(x => x.CanCreateSchoolInDistrictAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        auth.Setup(x => x.CanManageSchoolAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return auth.Object;
    }

    public static IOptions<JwtSettings> JwtOptions() =>
        Options.Create(new JwtSettings
        {
            Key = "test-key-that-is-long-enough-for-hmac-sha256",
            Issuer = "tests",
            Audience = "tests",
            RefreshTokenExpirationDays = 7
        });
}
