using FacilityOS.Application.DTOs.Schools;
using MediatR;

namespace FacilityOS.Application.Features.Schools.GetSchoolById;

public record GetSchoolByIdQuery(int Id) : IRequest<SchoolResponse>;