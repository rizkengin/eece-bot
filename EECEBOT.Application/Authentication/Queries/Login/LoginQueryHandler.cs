using EECEBOT.Application.Authentication.ResultModels;
using EECEBOT.Application.Common.AuthenticationServices.IdentityService;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.Authentication.Queries.Login;

internal sealed class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IIdentityService _identityService;

    public LoginQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var loginResult = await _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
        
        return loginResult.Match<ErrorOr<AuthenticationResult>>(
            success => success,
            failure => failure);
    }
}