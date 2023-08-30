using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.Exceptions;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NamingConvention;

namespace Identity.Application.Users.Commands;

public class CreateUserCommand : IRequest<string>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; }= null!;
    public string? Role { get; set; }
}

public class  CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly UserManager<User> _userManager;
    private readonly IIdentityDbContext _ctx;

    
    public CreateUserCommandHandler(UserManager<User> userManager, IIdentityDbContext ctx)
    {
        _userManager = userManager;
        _ctx = ctx;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.UserName,
        };
        
        

        var userCreateRes = await _userManager.CreateAsync(user, request.Password);
        
        if (!userCreateRes.Succeeded)
            throw new CustomException(ExMsg.User.UserNotCreated());

        var res = await _userManager.AddToRoleAsync(user,
            string.IsNullOrEmpty(request.Role) ? Roles.SimpleUser : request.Role);
        
        if (!res.Succeeded)
            throw new CustomException(ExMsg.User.UserNotCreated());


        await _ctx.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}


