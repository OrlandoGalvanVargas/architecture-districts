using FacilityOS.API.DTOs.Faculties;
using FluentValidation;

namespace FacilityOS.API.Features.Faculties.Validators;

public class CreateFacultyRequestValidator : AbstractValidator<CreateFacultyRequest>
{
    public CreateFacultyRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required")
            .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required")
            .MaximumLength(100).WithMessage("LastName must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("PhoneNumber must not exceed 20 characters")
            .Matches(@"^\+?[0-9\-\(\)\s]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("PhoneNumber format is invalid");

        RuleFor(x => x.Title)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Department)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Department))
            .WithMessage("Department must not exceed 100 characters");

        RuleFor(x => x)
            .Must(x => !(x.DistrictId.HasValue && x.SchoolId.HasValue))
            .WithMessage("Faculty can only be assigned to one entity (District or School).");

        RuleFor(x => x)
            .Must(x => x.DistrictId.HasValue || x.SchoolId.HasValue)
            .WithMessage("Faculty must be assigned to a District or School.");

        RuleFor(x => x.DistrictId)
            .GreaterThan(0).When(x => x.DistrictId.HasValue)
            .WithMessage("DistrictId must be greater than 0");

        RuleFor(x => x.SchoolId)
            .GreaterThan(0).When(x => x.SchoolId.HasValue)
            .WithMessage("SchoolId must be greater than 0");

        RuleFor(x => x.BeaconId)
            .GreaterThan(0).When(x => x.BeaconId.HasValue)
            .WithMessage("BeaconId must be greater than 0");
    }
}

public class UpdateFacultyRequestValidator : AbstractValidator<UpdateFacultyRequest>
{
    public UpdateFacultyRequestValidator()
    {
        Include(new CreateFacultyRequestValidator());
    }
}
