using FacilityOS.API.Data;
using FacilityOS.Domain.Models;
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
            var district = new District("Test District", "TD001", "CA", "Los Angeles", "90012", "123 Main St");
            context.Districts.Add(district);
            await context.SaveChangesAsync();
            return district;
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesSchoolSuccessfully()
        {
            
            using var context = CreateInMemoryContext();
            var district = await SeedDistrict(context);
            var handler = new CreateSchoolHandler(context, TestDoubles.AllowAllResources());

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

            
            var result = await handler.Handle(new CreateSchoolCommand(request), CancellationToken.None);

            
            Assert.NotNull(result);
            Assert.Equal("Lincoln Elementary", result.Name);
            Assert.Equal("LNE001", result.SchoolCode);
            Assert.Equal(SchoolLevel.Elementary, result.Level);
            Assert.Equal(district.Id, result.DistrictId);
            Assert.True(result.IsActive);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task Handle_InvalidDistrictId_ThrowsNotFoundException()
        {
            
            using var context = CreateInMemoryContext();
            var handler = new CreateSchoolHandler(context, TestDoubles.AllowAllResources());

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
                DistrictId = 999 
            };

            
            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.Handle(new CreateSchoolCommand(request), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DuplicateSchoolCode_ThrowsConflictException()
        {
            
            using var context = CreateInMemoryContext();
            var district = await SeedDistrict(context);

            context.Schools.Add(new School(
                "Existing School", "DUPLICATE001", SchoolLevel.High, SchoolType.Public,
                "789 Oak St", "Los Angeles", "CA", "90014", district.Id, 800));
            await context.SaveChangesAsync();

            var handler = new CreateSchoolHandler(context, TestDoubles.AllowAllResources());

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

            
            await Assert.ThrowsAsync<ConflictException>(
                () => handler.Handle(new CreateSchoolCommand(request), CancellationToken.None));
        }
    }
}
