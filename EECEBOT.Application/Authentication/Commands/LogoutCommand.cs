using EECEBOT.Application.Authentication.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Authentication.Commands;

public sealed record LogoutCommand(
    string Token,
    string RefreshToken) : IRequest<ErrorOr<LogoutResult>>;