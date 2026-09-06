using FacilityOS.Application.DTOs.Auth;
using FacilityOS.Application.DTOs.Schools;
using FacilityOS.Application.Features.Auth.Validators;
using FacilityOS.Application.Features.Schools.Validators;
using FacilityOS.Domain.Models.Enums;

namespace FacilityOS.API.Tests.Validation
{
    public class ValidatorsTests
    {
        [Fact]
        public void LoginRequestValidator_ShouldRejectInvalidEmail()
        {
            var validator = new LoginRequestValidator();

            var result = validator.Validate(new LoginRequest
            {
                Email = "not-an-email",
                Password = "Password123"
            });

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Email");
        }

        [Fact]
        public void CreateSchoolRequestValidator_ShouldRejectInvalidStateAndDistrict()
        {
            var validator = new CreateSchoolRequestValidator();

            var result = validator.Validate(new CreateSchoolRequest
            {
                Name = "Northview School",
                SchoolCode = "NVS-001",
                Level = SchoolLevel.Elementary,
                Type = SchoolType.Public,
                Address = "123 Main St",
                City = "Springfield",
                State = "USA",
                ZipCode = "123",
                DistrictId = 0
            });

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "State");
            Assert.Contains(result.Errors, error => error.PropertyName == "ZipCode");
            Assert.Contains(result.Errors, error => error.PropertyName == "DistrictId");
        }
    }
}
