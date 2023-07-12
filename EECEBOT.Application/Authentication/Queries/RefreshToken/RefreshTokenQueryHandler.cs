using EECEBOT.Application.Authentication.ResultModels;
using EECEBOT.Application.Common.AuthenticationServices.JwtTokenProvider;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Authentication.Queries.RefreshToken;

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, ErrorOr<RefreshTokenResult>>
{
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public RefreshTokenQueryHandler(IJwtTokenProvider jwtTokenProvider)
    {
        _jwtTokenProvider = jwtTokenProvider;
    }

    public async Task<ErrorOr<RefreshTokenResult>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var result = await _jwtTokenProvider.RefreshJwtToken(request.Token, request.RefreshToken, cancellationToken);

        return result.Match<ErrorOr<RefreshTokenResult>>(
            success => success,
            failure => failure);
    }
}