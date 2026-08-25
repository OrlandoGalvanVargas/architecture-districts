using FacilityOS.API.DTOs.Schools;
using MediatR;

namespace FacilityOS.API.Features.Schools.CreateSchool;
public record CreateSchoolCommand(CreateSchoolRequest Request) : IRequest<SchoolResponse>;