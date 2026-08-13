using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Features.Schools.CreateSchool;
using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Tests.Features.Schools
{
    public class CreateSchoolHandlerTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private static async Task<District> SeedDistrict(ApplicationDbContext context)
        {
            var district = new District
            {
                Name = "Test District",
                Code = "TD001",
                State = "CA",
                City = "Los Angeles",
                ZipCode = "90012",
                Address = "123 Main St"
            };
            context.Districts.Add(district);
            await context.SaveChangesAsync();
            return district;
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesSchoolSuccessfully()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var district = await SeedDistrict(context);
            var handler = new CreateSchoolHandler(context);

            var request = new CreateSchoolRequest
            {
                Name = "Lincoln Elementary",
                SchoolCode = "LNE001",
                Level = SchoolLevel.Elementary,
                Type = SchoolType.Public,
                Address = "456 Elm St",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90013",
                StudentCapacity = 500,
                DistrictId = district.Id
            };

            // Act
            var result = await handler.Handle(new CreateSchoolCommand(request), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Lincoln Elementary", result.Name);
            Assert.Equal("LNE001", result.SchoolCode);
            Assert.Equal("Elementary", result.Level);
            Assert.Equal(district.Id, result.DistrictId);
            Assert.True(result.IsActive);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task Handle_InvalidDistrictId_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var handler = new CreateSchoolHandler(context);

            var request = new CreateSchoolRequest
            {
                Name = "Lincoln Elementary",
                SchoolCode = "LNE001",
                Level = SchoolLevel.Elementary,
                Type = SchoolType.Public,
                Address = "456 Elm St",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90013",
                StudentCapacity = 500,
                DistrictId = 999 // no existe
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(new CreateSchoolCommand(request), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DuplicateSchoolCode_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var district = await SeedDistrict(context);

            context.Schools.Add(new School
            {
                Name = "Existing School",
                SchoolCode = "DUPLICATE001",
                Level = SchoolLevel.High,
                Type = SchoolType.Public,
                Address = "789 Oak St",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90014",
                StudentCapacity = 800,
                DistrictId = district.Id
            });
            await context.SaveChangesAsync();

            var handler = new CreateSchoolHandler(context);

            var request = new CreateSchoolRequest
            {
                Name = "New School",
                SchoolCode = "DUPLICATE001",
                Level = SchoolLevel.Elementary,
                Type = SchoolType.Charter,
                Address = "321 Pine St",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90015",
                StudentCapacity = 300,
                DistrictId = district.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(new CreateSchoolCommand(request), CancellationToken.None));
        }
    }
}
