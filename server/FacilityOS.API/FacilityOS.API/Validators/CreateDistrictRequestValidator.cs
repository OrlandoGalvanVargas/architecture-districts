using FacilityOS.API.DTOs.Districts;
using FluentValidation;

namespace FacilityOS.API.Validators;

public class CreateDistrictRequestValidator : AbstractValidator<CreateDistrictRequest>
{
    public CreateDistrictRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("Code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .Length(2).WithMessage("State must be exactly 2 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZipCode is required")
            .Matches(@"^\d{5}$").WithMessage("ZipCode must be 5 digits");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}