using FacilityOS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Tests.Features.Districts
{
    public class CreateDistrictHandlerTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesDistrictSuccessfully()
        {
            
            using var context = CreateInMemoryContext();
            var handler = new CreateDistrictHandler(context, TestDoubles.AdminUser());

            var request = new CreateDistrictRequest
            {
                Name = "Test District",
                Code = "TEST001",
                State = "CA",
                City = "Los Angeles",
                ZipCode = "90012",
                Address = "123 Main St",
                Description = "A test district"
            };

            
            var result = await handler.Handle(new CreateDistrictCommand(request), CancellationToken.None);

            
            Assert.NotNull(result);
            Assert.Equal("Test District", result.Name);
            Assert.Equal("TEST001", result.Code);
            Assert.True(result.Id > 0);

            var savedDistrict = await context.Districts.FirstOrDefaultAsync(d => d.Code == "TEST001");
            Assert.NotNull(savedDistrict);
        }

        [Fact]
        public async Task Handle_DuplicateCode_ThrowsConflictException()
        {
            
            using var context = CreateInMemoryContext();
            context.Districts.Add(new District("Existing District", "DUPLICATE001", "CA", "San Diego", "92101", "456 Elm St"));
            await context.SaveChangesAsync();

            var handler = new CreateDistrictHandler(context, TestDoubles.AdminUser());

            var request = new CreateDistrictRequest
            {
                Name = "New District",
                Code = "DUPLICATE001", 
                State = "NY",
                City = "New York",
                ZipCode = "10001",
                Address = "789 Oak St"
            };

            
            await Assert.ThrowsAsync<ConflictException>(
                () => handler.Handle(new CreateDistrictCommand(request), CancellationToken.None));
        }
    }
}
