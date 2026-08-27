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

            var originalDistrict = new District
            {
                Name = "Original District",
                Code = "OD-001",
                State = "CA",
                City = "Los Angeles",
                ZipCode = "90001",
                Address = "1 Main St"
            };

            var newDistrict = new District
            {
                Name = "New District",
                Code = "ND-001",
                State = "TX",
                City = "Austin",
                ZipCode = "73301",
                Address = "2 Main St"
            };

            context.Districts.AddRange(originalDistrict, newDistrict);
            await context.SaveChangesAsync();

            var school = new School
            {
                Name = "Old School",
                SchoolCode = "OLD-001",
                Level = SchoolLevel.Elementary,
                Type = SchoolType.Public,
                Address = "100 Old St",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                StudentCapacity = 300,
                IsActive = true,
                DistrictId = originalDistrict.Id
            };

            context.Schools.Add(school);
            await context.SaveChangesAsync();

            var handler = new UpdateSchoolHandler(context);

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
            Assert.Equal(SchoolLevel.High.ToString(), result!.Level);
            Assert.Equal(SchoolType.Charter.ToString(), result.Type);
            Assert.Equal(newDistrict.Id, result.DistrictId);
            Assert.Equal(0, await context.Districts.FirstAsync(d => d.Id == originalDistrict.Id).ContinueWith(t => t.Result.SchoolCount));
            Assert.Equal(1, await context.Districts.FirstAsync(d => d.Id == newDistrict.Id).ContinueWith(t => t.Result.SchoolCount));
        }

        [Fact]
        public async Task CreateAndDeleteSchool_KeepDistrictCountsInSync()
        {
            using var context = CreateInMemoryContext();

            var district = new District
            {
                Name = "District A",
                Code = "DA-001",
                State = "CA",
                City = "Los Angeles",
                ZipCode = "90001",
                Address = "1 Main St"
            };

            context.Districts.Add(district);
            await context.SaveChangesAsync();

            var createHandler = new CreateSchoolHandler(context);
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

            Assert.Equal(1, await context.Districts.FirstAsync(d => d.Id == district.Id).ContinueWith(t => t.Result.SchoolCount));

            var deleteHandler = new DeleteSchoolHandler(context);
            var deleted = await deleteHandler.Handle(new DeleteSchoolCommand(created.Id), CancellationToken.None);

            Assert.True(deleted);
            Assert.Equal(0, await context.Districts.FirstAsync(d => d.Id == district.Id).ContinueWith(t => t.Result.SchoolCount));
        }
    }
}
