using FacilityOS.API.Common;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models.Enums;
using FluentValidation;

namespace FacilityOS.API.Features.Users.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => role == AppConstants.Roles.Admin ||
                          role == AppConstants.Roles.DistrictAdmin ||
                          role == AppConstants.Roles.SchoolAdmin)
            .WithMessage($"Role must be one of: {AppConstants.Roles.Admin}, {AppConstants.Roles.DistrictAdmin}, {AppConstants.Roles.SchoolAdmin}");

        RuleFor(x => x)
            .Must(x => !(x.Role == AppConstants.Roles.Admin && x.EntityType != UserEntityType.Global))
            .WithMessage("Global Admin role must have Global entity type.");

        RuleFor(x => x)
            .Must(x => !(x.EntityType != UserEntityType.Global && !x.EntityId.HasValue))
            .WithMessage("EntityId is required when EntityType is District or School.");
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => role == AppConstants.Roles.Admin ||
                          role == AppConstants.Roles.DistrictAdmin ||
                          role == AppConstants.Roles.SchoolAdmin)
            .WithMessage($"Role must be one of: {AppConstants.Roles.Admin}, {AppConstants.Roles.DistrictAdmin}, {AppConstants.Roles.SchoolAdmin}");

        RuleFor(x => x)
            .Must(x => !(x.Role == AppConstants.Roles.Admin && x.EntityType != UserEntityType.Global))
            .WithMessage("Global Admin role must have Global entity type.");

        RuleFor(x => x)
            .Must(x => !(x.EntityType != UserEntityType.Global && !x.EntityId.HasValue))
            .WithMessage("EntityId is required when EntityType is District or School.");

        When(x => !string.IsNullOrEmpty(x.NewPassword), () =>
        {
            RuleFor(x => x.NewPassword)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number");
        });
    }
}