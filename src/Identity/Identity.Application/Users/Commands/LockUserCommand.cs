using Identity.Application.Infrastructure.Exceptions;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Users.Commands;

public class LockUserCommand : IRequest<string>
{
    public string UserId { get; set; } = null!;
}

public class LockUserCommandHandler : IRequestHandler<LockUserCommand, string>
{
    private readonly UserManager<User> _userManager;

    public LockUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }


    public async Task<string> Handle(LockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager
            .FindByIdAsync(request.UserId) ?? throw new CustomException(ExMsg.User.NotFound());

        if (!user.LockoutEnabled)
        {
            user.LockoutEnabled = true;
        }
        user.LockoutEnd = DateTimeOffset.MaxValue;

        await _userManager.UpdateAsync(user);
        await _userManager.UpdateSecurityStampAsync(user);

        return user.Id;
    }
}