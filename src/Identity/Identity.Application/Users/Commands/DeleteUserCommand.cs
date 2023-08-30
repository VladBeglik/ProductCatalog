using Identity.Application.Infrastructure.Exceptions;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Users.Commands;

public class DeleteUserCommand : IRequest
{
    public string UserId { get; set; } = null!;
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<User> _userManager;

    public DeleteUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager
            .FindByIdAsync(request.UserId) ?? throw new CustomException(ExMsg.User.NotFound());

        var res = await _userManager.DeleteAsync(user);

        if (!res.Succeeded)
        {
            throw new CustomException(ExMsg.User.UserNotDeleted());
        }
        
        return Unit.Value;
    }
}