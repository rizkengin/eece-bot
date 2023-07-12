using EECEBOT.Application.Authentication.Commands;
using EECEBOT.Application.Authentication.Queries.Login;
using EECEBOT.Application.Authentication.Queries.RefreshToken;
using EECEBOT.Contracts.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EECEBOT.API.Controllers;

[AllowAnonymous]
[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public AuthenticationController(
        ISender sender,
        IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var query = _mapper.Map<LoginQuery>(request);
        
        var loginResult = await _sender.Send(query);
        
        return loginResult.Match(
            success => Ok(_mapper.Map<AuthenticationResponse>(success)),
            Problem);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest refreshTokenRequest)
    {
        var query = _mapper.Map<RefreshTokenQuery>(refreshTokenRequest);

        var refreshResult = await _sender.Send(query);

        return refreshResult.Match(
            success => Ok(_mapper.Map<RefreshTokenResponse>(success)),
            Problem);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest logoutRequest)
    {
        var command = _mapper.Map<LogoutCommand>(logoutRequest);

        var logoutResult = await _sender.Send(command);

        return logoutResult.Match(
            success => Ok(_mapper.Map<LogoutResponse>(success)),
            Problem);
    }
}