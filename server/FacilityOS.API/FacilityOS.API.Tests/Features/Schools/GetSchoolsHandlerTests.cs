using FacilityOS.API.Data;
using FacilityOS.Domain.Models;
using Microsoft.EntityFrameworkCore;

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
            var d1 = new District("District 1", "D001", "CA", "LA", "90001", "1 Main");
            var d2 = new District("District 2", "D002", "NY", "NYC", "10001", "2 Main");
            context.Districts.AddRange(d1, d2);
            await context.SaveChangesAsync();

            context.Schools.AddRange(
                new School("Alpha Elementary", "AE001", SchoolLevel.Elementary, SchoolType.Public, "1 St", "LA", "CA", "90001", d1.Id, 300),
                new School("Beta High", "BH001", SchoolLevel.High, SchoolType.Charter, "2 St", "LA", "CA", "90002", d1.Id, 800),
                new School("Gamma Middle", "GM001", SchoolLevel.Middle, SchoolType.Public, "3 St", "NYC", "NY", "10001", d2.Id, 500)
            );
            context.Schools.Local.Last().Deactivate();
            await context.SaveChangesAsync();
            return (d1, d2);
        }

        [Fact]
        public async Task Handle_NoFilters_ReturnsAllSchoolsPaged()
        {
            
            using var context = CreateInMemoryContext();
            await SeedData(context);
            var handler = new GetSchoolsHandler(context, TestDoubles.AdminUser());

            
            var result = await handler.Handle(
                new GetSchoolsQuery(null, null, null, null, null, Page: 1, PageSize: 10),
                CancellationToken.None);

            
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(1, result.TotalPages);
        }

        [Fact]
        public async Task Handle_FilterByDistrictId_ReturnsOnlyThatDistrictsSchools()
        {
            
            using var context = CreateInMemoryContext();
            var (d1, _) = await SeedData(context);
            var handler = new GetSchoolsHandler(context, TestDoubles.AdminUser());

            
            var result = await handler.Handle(
                new GetSchoolsQuery(d1.Id, null, null, null, null, Page: 1, PageSize: 10),
                CancellationToken.None);

            
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, s => Assert.Equal(d1.Id, s.DistrictId));
        }

        [Fact]
        public async Task Handle_FilterByIsActive_ReturnsOnlyActiveSchools()
        {
            
            using var context = CreateInMemoryContext();
            await SeedData(context);
            var handler = new GetSchoolsHandler(context, TestDoubles.AdminUser());

            
            var result = await handler.Handle(
                new GetSchoolsQuery(null, null, null, null, true, Page: 1, PageSize: 10),
                CancellationToken.None);

            
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, s => Assert.True(s.IsActive));
        }

        [Fact]
        public async Task Handle_Pagination_ReturnsCorrectPage()
        {
            
            using var context = CreateInMemoryContext();
            await SeedData(context);
            var handler = new GetSchoolsHandler(context, TestDoubles.AdminUser());

            
            var page1 = await handler.Handle(
                new GetSchoolsQuery(null, null, null, null, null, Page: 1, PageSize: 2),
                CancellationToken.None);

            var page2 = await handler.Handle(
                new GetSchoolsQuery(null, null, null, null, null, Page: 2, PageSize: 2),
                CancellationToken.None);

            
            Assert.Equal(3, page1.TotalCount);
            Assert.Equal(2, page1.Items.Count);
            Assert.Equal(2, page1.TotalPages);
            Assert.True(page1.HasNext);
            Assert.False(page1.HasPrevious);

            Assert.Single(page2.Items);
            Assert.False(page2.HasNext);
            Assert.True(page2.HasPrevious);
        }
    }
}
