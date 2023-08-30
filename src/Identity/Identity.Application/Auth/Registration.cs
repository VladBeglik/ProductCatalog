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

public class Registration : IRequest<IActionResult>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegistrationHandler : IRequestHandler<Registration, IActionResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IClock _clock;
    private readonly ITokenService _tokenService;

    public RegistrationHandler(UserManager<User> userManager, IConfiguration configuration, IClock clock, ITokenService tokenService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _clock = clock;
        _tokenService = tokenService;
    }

    public async Task<IActionResult> Handle(Registration request, CancellationToken cancellationToken)
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