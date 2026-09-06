using FacilityOS.Application.DTOs.Beacons;
using FacilityOS.Application.DTOs.Faculties;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Application.Features.Beacons.Validators;
using FacilityOS.Application.Features.Faculties.Validators;
using FacilityOS.Application.Features.Users.Validators;
using FacilityOS.Domain.Models.Enums;

namespace FacilityOS.API.Tests.Validation;

public class FinalizedValidatorsTests
{
    [Fact]
    public void CreateBeaconValidator_RejectsConflictingAssignmentsAndInvalidSerial()
    {
        var result = new CreateBeaconRequestValidator().Validate(new CreateBeaconRequest
        {
            DeviceName = "Front door",
            SerialNumber = "invalid serial",
            Type = BeaconType.Pendant,
            DistrictId = 1,
            SchoolId = 2
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "SerialNumber");
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("assigned to one entity"));
    }

    [Fact]
    public void UpdateBeaconValidator_AcceptsValidUnassignedBeacon()
    {
        var result = new UpdateBeaconRequestValidator().Validate(new UpdateBeaconRequest
        {
            DeviceName = "Updated beacon",
            Type = BeaconType.Mobile,
            Status = BeaconStatus.Available
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateFacultyValidator_RequiresExactlyOneAssignment()
    {
        var validator = new CreateFacultyRequestValidator();
        var valid = validator.Validate(new CreateFacultyRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            SchoolId = 10
        });
        var invalid = validator.Validate(new CreateFacultyRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            DistrictId = 1,
            SchoolId = 10
        });

        Assert.True(valid.IsValid);
        Assert.False(invalid.IsValid);
        Assert.Contains(invalid.Errors, error => error.ErrorMessage.Contains("one entity"));
    }

    [Fact]
    public void CreateUserValidator_EnforcesPasswordRoleAndScopeRules()
    {
        var result = new CreateUserRequestValidator().Validate(new CreateUserRequest
        {
            Name = "Scoped user",
            Email = "user@example.com",
            Password = "weak",
            Role = "Admin",
            EntityType = UserEntityType.District
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Password");
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("Global Admin"));
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("EntityId"));
    }

    [Fact]
    public void UpdateUserValidator_AllowsOmittedOptionalPassword()
    {
        var result = new UpdateUserRequestValidator().Validate(new UpdateUserRequest
        {
            Name = "Updated user",
            Email = "user@example.com",
            Role = "SchoolAdmin",
            EntityType = UserEntityType.School,
            EntityId = 5
        });

        Assert.True(result.IsValid);
    }
}
