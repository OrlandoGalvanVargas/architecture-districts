using FacilityOS.API.DTOs.Beacons;
using FluentValidation;

namespace FacilityOS.API.Features.Beacons.Validators;

public class CreateBeaconRequestValidator : AbstractValidator<CreateBeaconRequest>
{
    public CreateBeaconRequestValidator()
    {
        RuleFor(x => x.DeviceName)
            .NotEmpty().WithMessage("DeviceName is required")
            .MaximumLength(100).WithMessage("DeviceName must not exceed 100 characters");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("SerialNumber is required")
            .MaximumLength(50).WithMessage("SerialNumber must not exceed 50 characters")
            .Matches(@"^[A-Za-z0-9-_]+$").WithMessage("SerialNumber must contain only letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type is invalid");

        RuleFor(x => x)
            .Must(x => !(x.DistrictId.HasValue && x.SchoolId.HasValue))
            .WithMessage("Beacon can only be assigned to one entity (District or School) at birth.");

        RuleFor(x => x.DistrictId)
            .GreaterThan(0).When(x => x.DistrictId.HasValue)
            .WithMessage("DistrictId must be greater than 0");

        RuleFor(x => x.SchoolId)
            .GreaterThan(0).When(x => x.SchoolId.HasValue)
            .WithMessage("SchoolId must be greater than 0");
    }
}

public class UpdateBeaconRequestValidator : AbstractValidator<UpdateBeaconRequest>
{
    public UpdateBeaconRequestValidator()
    {
        RuleFor(x => x.DeviceName)
            .NotEmpty().WithMessage("DeviceName is required")
            .MaximumLength(100).WithMessage("DeviceName must not exceed 100 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type is invalid");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status is invalid");

        RuleFor(x => x)
            .Must(x => !(x.DistrictId.HasValue && x.SchoolId.HasValue))
            .WithMessage("Beacon can only be assigned to one entity (District or School).");

        RuleFor(x => x.DistrictId)
            .GreaterThan(0).When(x => x.DistrictId.HasValue)
            .WithMessage("DistrictId must be greater than 0");

        RuleFor(x => x.SchoolId)
            .GreaterThan(0).When(x => x.SchoolId.HasValue)
            .WithMessage("SchoolId must be greater than 0");
    }
}
