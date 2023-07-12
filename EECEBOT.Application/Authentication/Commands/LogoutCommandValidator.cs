using FluentValidation;

namespace EECEBOT.Application.Authentication.Commands;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}