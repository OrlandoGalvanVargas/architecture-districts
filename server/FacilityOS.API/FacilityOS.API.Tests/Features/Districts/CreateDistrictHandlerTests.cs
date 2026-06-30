using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Features.Districts.CreateDistrict;
using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacilityOS.API.Tests.Features.Districts
{
    public class CreateDistrictHandlerTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // BD nueva y aislada por test
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesDistrictSuccessfully()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var handler = new CreateDistrictHandler(context);

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

            // Act
            var result = await handler.Handle(new CreateDistrictCommand(request), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test District", result.Name);
            Assert.Equal("TEST001", result.Code);
            Assert.True(result.Id > 0);

            var savedDistrict = await context.Districts.FirstOrDefaultAsync(d => d.Code == "TEST001");
            Assert.NotNull(savedDistrict);
        }

        [Fact]
        public async Task Handle_DuplicateCode_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Districts.Add(new District
            {
                Name = "Existing District",
                Code = "DUPLICATE001",
                State = "CA",
                City = "San Diego",
                ZipCode = "92101",
                Address = "456 Elm St"
            });
            await context.SaveChangesAsync();

            var handler = new CreateDistrictHandler(context);

            var request = new CreateDistrictRequest
            {
                Name = "New District",
                Code = "DUPLICATE001", // mismo código
                State = "NY",
                City = "New York",
                ZipCode = "10001",
                Address = "789 Oak St"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(new CreateDistrictCommand(request), CancellationToken.None));
        }
    }
}
