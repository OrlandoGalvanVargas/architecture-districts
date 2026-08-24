using MediatR;

namespace FacilityOS.API.Features.Schools.DeleteSchool
{
    public record DeleteSchoolCommand(int Id) : IRequest;
}
