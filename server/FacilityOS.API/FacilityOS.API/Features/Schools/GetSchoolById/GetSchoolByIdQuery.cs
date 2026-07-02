using FacilityOS.API.DTOs.Schools;
using MediatR;

namespace FacilityOS.API.Features.Schools.GetSchoolById
{
    public record GetSchoolByIdQuery(int Id) : IRequest<SchoolResponse?>;
}
