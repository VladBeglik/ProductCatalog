using Identity.Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

public class AuthController : MediatrController
{

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand model)
    {
        var result = await Mediator.Send(model);
        return result;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationCommand model)
    {
        var result = await Mediator.Send(model);
        return result;
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegistrationAdminCommand model)
    {
        var result = await Mediator.Send(model);
        return result;
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand tokenModel)
    {
        var result = await Mediator.Send(tokenModel);
        return result;
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string userId)
    {
        await Mediator.Send(new RevokeCommand{UserId = userId});
        return NoContent();
    }

    [Authorize]
    [HttpPost]
    [Route("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        await Mediator.Send(new RevokeAllCommand());
        return NoContent();
    }
}