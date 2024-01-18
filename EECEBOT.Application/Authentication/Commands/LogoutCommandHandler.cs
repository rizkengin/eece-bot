using EECEBOT.Application.Authentication.ResultModels;
using EECEBOT.Application.Common.AuthenticationServices.JwtTokenProvider;
using EECEBOT.Application.Common.Persistence;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Authentication.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ErrorOr<LogoutResult>>
{
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IJwtTokenProvider jwtTokenProvider,
        IUnitOfWork unitOfWork)
    {
        _jwtTokenProvider = jwtTokenProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<LogoutResult>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var revokeRefreshTokenResult = await _jwtTokenProvider
            .RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (revokeRefreshTokenResult.IsError)
        {
            return revokeRefreshTokenResult.Errors;
        }

        var user = revokeRefreshTokenResult.Value;
        
        await _unitOfWork.UpdateAsync(user, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LogoutResult(true);
    }
}