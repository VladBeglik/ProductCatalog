using Microsoft.AspNetCore.Identity;
using NodaTime;

namespace Identity.Domain;

public class User : IdentityUser
{
    public string? RefreshToken { get; set; }
    public LocalDateTime RefreshTokenExpiryTime { get; set; }
    public int Role { get; set; } 
}