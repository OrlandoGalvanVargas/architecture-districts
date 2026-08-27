using FacilityOS.Application.DTOs.Schools;
using MediatR;

namespace FacilityOS.Application.Features.Schools.CreateSchool;

public record CreateSchoolCommand(CreateSchoolRequest Request) : IRequest<SchoolResponse>;