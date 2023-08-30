using System.Net.NetworkInformation;
using Identity.Application.Infrastructure.Exceptions;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NamingConvention;

namespace Identity.Application.Auth;

public class RegistrationAdminCommand : IRequest<IActionResult>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegistrationAdminHandler : IRequestHandler<RegistrationAdminCommand, IActionResult>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegistrationAdminHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;

    }

    public async Task<IActionResult> Handle(RegistrationAdminCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.FindByNameAsync(request.Username);
        if (userExists != null)
            throw new CustomException();

        var user = new User
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Username,
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
             throw new CustomException();

        var res = await _userManager.AddToRoleAsync(user, Roles.Admin);
        
        if (!res.Succeeded)
            throw new CustomException();

        
        return new ObjectResult( new { Status = "Success", Message = "User created successfully!" });
    }
}