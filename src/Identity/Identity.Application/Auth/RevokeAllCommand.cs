using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Auth;

public class RevokeAllCommand : IRequest
{
    
}

public class RevokeAllCommandHandler : IRequestHandler<RevokeAllCommand>
{
    private readonly UserManager<User> _userManager;

    public RevokeAllCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(RevokeAllCommand request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();
        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
        
        return Unit.Value;
    }
}