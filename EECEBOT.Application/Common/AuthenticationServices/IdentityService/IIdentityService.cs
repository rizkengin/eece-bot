using EECEBOT.Application.Authentication.ResultModels;
using ErrorOr;

namespace EECEBOT.Application.Common.AuthenticationServices.IdentityService;

public interface IIdentityService
{
    Task<ErrorOr<AuthenticationResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}