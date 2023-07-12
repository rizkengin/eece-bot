using EECEBOT.Application.Authentication.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Authentication.Queries.RefreshToken;

public sealed record RefreshTokenQuery(string Token,
    string RefreshToken) : IRequest<ErrorOr<RefreshTokenResult>>;