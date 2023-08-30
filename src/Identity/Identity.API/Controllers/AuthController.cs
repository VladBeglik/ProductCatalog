using Identity.Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

public class AuthController : MediatrController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        var result = await _mediator.Send(model);
        return result;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] Registration model)
    {
        var result = await _mediator.Send(model);
        return result;
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
        var result = await _mediator.Send(new RegisterAdminCommand(model));
        return result;
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(tokenModel));
        return result;
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var result = await _mediator.Send(new RevokeCommand(username));
        return result;
    }

    [Authorize]
    [HttpPost]
    [Route("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        var result = await _mediator.Send(new RevokeAllCommand());
        return result;
    }
}