using FacilityOS.API.DTOs.Auth;
using FluentValidation;

namespace FacilityOS.API.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required");
    }
}