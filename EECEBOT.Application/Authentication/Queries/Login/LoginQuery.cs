using EECEBOT.Application.Authentication.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Authentication.Queries.Login;

public sealed record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;