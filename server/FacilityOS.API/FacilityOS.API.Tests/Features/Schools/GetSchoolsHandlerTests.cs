using FacilityOS.API.Data;
using FacilityOS.API.Features.Schools.GetSchools;
using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacilityOS.API.Tests.Features.Schools
{
    public class GetSchoolsHandlerTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private static async Task<(District d1, District d2)> SeedData(ApplicationDbContext context)
        {
            var d1 = new District { Name = "District 1", Code = "D001", State = "CA", City = "LA", ZipCode = "90001", Address = "1 Main" };
            var d2 = new District { Name = "District 2", Code = "D002", State = "NY", City = "NYC", ZipCode = "10001", Address = "2 Main" };
            context.Districts.AddRange(d1, d2);
            await context.SaveChangesAsync();

            context.Schools.AddRange(
                new School { Name = "Alpha Elementary", SchoolCode = "AE001", Level = SchoolLevel.Elementary, Type = SchoolType.Public, Address = "1 St", City = "LA", State = "CA", ZipCode = "90001", StudenCapacity = 300, DistrictId = d1.Id, isActive = true },
                new School { Name = "Beta High", SchoolCode = "BH001", Level = SchoolLevel.High, Type = SchoolType.Charter, Address = "2 St", City = "LA", State = "CA", ZipCode = "90002", StudenCapacity = 800, DistrictId = d1.Id, isActive = true },
                new School { Name = "Gamma Middle", SchoolCode = "GM001", Level = SchoolLevel.Middle, Type = SchoolType.Public, Address = "3 St", City = "NYC", State = "NY", ZipCode = "10001", StudenCapacity = 500, DistrictId = d2.Id, isActive = false }
            );
            await context.SaveChangesAsync();
            return (d1, d2);
        }

        [Fact]
        public async Task Handle_NoFilters_ReturnsAllSchoolsPaged()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            await SeedData(context);
            var handler = new GetSchoolsHandler(context);

            // Act
            var result = await handler.Handle(
                new GetSchoolsQuery(null, null, null, null, Page: 1, PageSize: 10),
                CancellationToken.None);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(1, result.TotalPages);
        }

        [Fact]
        public async Task Handle_FilterByDistrictId_ReturnsOnlyThatDistrictsSchools()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var (d1, _) = await SeedData(context);
            var handler = new GetSchoolsHandler(context);

            // Act
            var result = await handler.Handle(
                new GetSchoolsQuery(d1.Id, null, null, null, Page: 1, PageSize: 10),
                CancellationToken.None);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, s => Assert.Equal(d1.Id, s.DistrictId));
        }

        [Fact]
        public async Task Handle_FilterByIsActive_ReturnsOnlyActiveSchools()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            await SeedData(context);
            var handler = new GetSchoolsHandler(context);

            // Act
            var result = await handler.Handle(
                new GetSchoolsQuery(null, null, null, true, Page: 1, PageSize: 10),
                CancellationToken.None);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, s => Assert.True(s.IsActive));
        }

        [Fact]
        public async Task Handle_Pagination_ReturnsCorrectPage()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            await SeedData(context);
            var handler = new GetSchoolsHandler(context);

            // Act — page 1 con pageSize 2 debe traer 2, page 2 debe traer 1
            var page1 = await handler.Handle(
                new GetSchoolsQuery(null, null, null, null, Page: 1, PageSize: 2),
                CancellationToken.None);

            var page2 = await handler.Handle(
                new GetSchoolsQuery(null, null, null, null, Page: 2, PageSize: 2),
                CancellationToken.None);

            // Assert
            Assert.Equal(3, page1.TotalCount);
            Assert.Equal(2, page1.Items.Count);
            Assert.Equal(2, page1.TotalPages);
            Assert.True(page1.HasNextPage);
            Assert.False(page1.HasPreviousPage);

            Assert.Equal(1, page2.Items.Count);
            Assert.False(page2.HasNextPage);
            Assert.True(page2.HasPreviousPage);
        }
    }
}
