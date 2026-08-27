using FacilityOS.Application.DTOs.Schools;
using MediatR;

namespace FacilityOS.Application.Features.Schools.UpdateSchool;

public record UpdateSchoolCommand(int Id, UpdateSchoolRequest Request) : IRequest<SchoolResponse>;