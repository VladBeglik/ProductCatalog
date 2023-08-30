using Identity.Application.Infrastructure.Exceptions;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Users.Commands;

public class UpdateUserPasswordCommand : IRequest
{
    public string UserId { get; set; } = null!;
    public string Password { get; set; } = null!;
}


public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand>
{
    private readonly UserManager<User> _userManager;

    public UpdateUserPasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager
            .FindByIdAsync(request.UserId);

        if (user == default )
            throw new CustomException(ExMsg.User.NotFound());

        await _userManager.RemovePasswordAsync(user);

        await _userManager.AddPasswordAsync(user, request.Password);

        await _userManager.UpdateSecurityStampAsync(user);
        return Unit.Value;
    }
}
