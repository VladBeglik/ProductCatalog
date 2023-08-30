using System.IdentityModel.Tokens.Jwt;
using Catalog.Application.Infrastructure;
using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.Exceptions;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Identity.Application.Auth;

public class RefreshTokenCommand : IRequest<IActionResult>
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IActionResult>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IClock _clock;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;


    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<User> userManager, IClock clock, ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _clock = clock;
        _logger = logger;
    }

    public async Task<IActionResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            _logger.LogError("Invalid client request");
            throw new CustomException("Invalid client request");
        }

        var accessToken = request.AccessToken;
        var refreshToken = request.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!);
        if (principal == null)
        {
            _logger.LogError("Invalid access token or refresh token");
            throw new CustomException("Invalid access token or refresh token");
        }

        var username = principal.Identity!.Name;


        var user = await _userManager.FindByNameAsync(username!);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= _clock.GetNow())
        {
            throw new CustomException("Invalid access token or refresh token");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList());
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }
}
