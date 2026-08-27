using MediatR;

namespace FacilityOS.Application.Features.Schools.DeleteSchool;

public record DeleteSchoolCommand(int Id) : IRequest;
