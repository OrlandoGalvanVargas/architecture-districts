using FacilityOS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Tests.Features.Schools
{
    public class UpdateSchoolHandlerTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Handle_UpdatesLevelTypeAndDistrictAndRecalculatesCounts()
        {
            using var context = CreateInMemoryContext();

            var originalDistrict = new District("Original District", "OD-001", "CA", "Los Angeles", "90001", "1 Main St");

            var newDistrict = new District("New District", "ND-001", "TX", "Austin", "73301", "2 Main St");

            context.Districts.AddRange(originalDistrict, newDistrict);
            await context.SaveChangesAsync();

            var school = new School(
                "Old School", "OLD-001", SchoolLevel.Elementary, SchoolType.Public,
                "100 Old St", "Los Angeles", "CA", "90001", originalDistrict.Id, 300);

            context.Schools.Add(school);
            await context.SaveChangesAsync();

            var handler = new UpdateSchoolHandler(context, TestDoubles.AllowAllResources());

            var result = await handler.Handle(new UpdateSchoolCommand(school.Id, new UpdateSchoolRequest
            {
                Name = "Updated School",
                SchoolCode = "NEW-001",
                Level = SchoolLevel.High,
                Type = SchoolType.Charter,
                Address = "200 New St",
                City = "Austin",
                State = "TX",
                ZipCode = "73301",
                StudentCapacity = 600,
                IsActive = false,
                DistrictId = newDistrict.Id
            }), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(SchoolLevel.High, result!.Level);
            Assert.Equal(SchoolType.Charter, result.Type);
            Assert.Equal(newDistrict.Id, result.DistrictId);
            Assert.Equal(0, await context.Schools.CountAsync(s => s.DistrictId == originalDistrict.Id));
            Assert.Equal(1, await context.Schools.CountAsync(s => s.DistrictId == newDistrict.Id));
        }

        [Fact]
        public async Task CreateAndDeleteSchool_KeepDistrictCountsInSync()
        {
            using var context = CreateInMemoryContext();

            var district = new District("District A", "DA-001", "CA", "Los Angeles", "90001", "1 Main St");

            context.Districts.Add(district);
            await context.SaveChangesAsync();

            var createHandler = new CreateSchoolHandler(context, TestDoubles.AllowAllResources());
            var created = await createHandler.Handle(new CreateSchoolCommand(new CreateSchoolRequest
            {
                Name = "New School",
                SchoolCode = "NS-001",
                Level = SchoolLevel.Elementary,
                Type = SchoolType.Public,
                Address = "2 Main St",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                StudentCapacity = 500,
                DistrictId = district.Id
            }), CancellationToken.None);

            Assert.Equal(1, await context.Schools.CountAsync(s => s.DistrictId == district.Id));

            var deleteHandler = new DeleteSchoolHandler(context, TestDoubles.AllowAllResources());
            await deleteHandler.Handle(new DeleteSchoolCommand(created.Id), CancellationToken.None);

            var deletedSchool = await context.Schools
                .IgnoreQueryFilters()
                .SingleAsync(s => s.Id == created.Id);
            Assert.False(deletedSchool.IsActive);
            Assert.True(deletedSchool.IsDeleted);
        }
    }
}
