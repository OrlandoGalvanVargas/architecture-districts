using FacilityOS.API.DTOs.Schools;
using FluentValidation;

namespace FacilityOS.API.Validators;

public class UpdateSchoolRequestValidator : AbstractValidator<UpdateSchoolRequest>
{
    public UpdateSchoolRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.SchoolCode)
            .NotEmpty().WithMessage("SchoolCode is required")
            .MaximumLength(50).WithMessage("SchoolCode must not exceed 50 characters");

        RuleFor(x => x.Level).IsInEnum().WithMessage("Level is invalid");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Type is invalid");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .Length(2).WithMessage("State must be 2 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZipCode is required")
            .Matches(@"^\d{5}$").WithMessage("ZipCode must be 5 digits");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
            .WithMessage("ContactEmail must be a valid email address");

        RuleFor(x => x.StudentCapacity)
            .InclusiveBetween(1, 10000).WithMessage("StudentCapacity must be between 1 and 10000");

        RuleFor(x => x.DistrictId)
            .GreaterThan(0).WithMessage("DistrictId is required");
    }
}