using FacilityOS.API.DTOs.Schools;
using MediatR;

namespace FacilityOS.API.Features.Schools.UpdateSchool
{
    public record UpdateSchoolCommand(int Id, UpdateSchoolRequest Request) : IRequest<SchoolResponse>;
}
