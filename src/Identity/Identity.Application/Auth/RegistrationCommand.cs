using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.Exceptions;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NodaTime;

namespace Identity.Application.Auth;

public class RegistrationCommand : IRequest<IActionResult>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegistrationHandler : IRequestHandler<RegistrationCommand, IActionResult>
{
    private readonly UserManager<User> _userManager;


    public RegistrationHandler(UserManager<User> userManager)
    {
        _userManager = userManager;

    }

    public async Task<IActionResult> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.FindByNameAsync(request.Username);
        if (userExists != null)
            throw new CustomException();

        var user = new User
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Username
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new CustomException(ExMsg.User.UserNotCreated());

        return new ObjectResult( new { Status = "Success", Message = "User created successfully!" });
    }
}