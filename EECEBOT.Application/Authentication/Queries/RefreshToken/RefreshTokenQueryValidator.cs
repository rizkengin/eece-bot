using FluentValidation;

namespace EECEBOT.Application.Authentication.Queries.RefreshToken;

public class RefreshTokenQueryValidator: AbstractValidator<RefreshTokenQuery>
{
    public RefreshTokenQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}