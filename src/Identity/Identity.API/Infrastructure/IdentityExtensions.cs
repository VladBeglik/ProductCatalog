using System.Reflection;
using System.Text;
using Identity.Application.Infrastructure;
using Identity.Domain;
using Identity.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Infrastructure;

public static class IdentityExtensions
{
    public static IServiceCollection AddCustomIdentity(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        var connectionString = configuration.GetConnectionString("SqlServer");
        var pathBase = configuration.GetValue<string>("PATH_BASE");

        var migrationsAssembly = typeof(IdentityDbContext).GetTypeInfo().Assembly.GetName().Name;

        services
            .AddIdentity<User, IdentityRole>(_ =>
            {
                _.Password.RequireDigit = false;
                _.Password.RequireUppercase = false;
                _.Password.RequireNonAlphanumeric = false;
                _.Password.RequiredLength = 4;
                _.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            
        .AddJwtBearer(options =>
        {
            options.SaveToken = false;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = "https://localhost:5001",
                ValidIssuer = "https://localhost:5001",
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey("JWTRefreshTokenHIGHsecuredPasswordVVVp1OH7Xzyr"u8.ToArray())
            };
        });
        
        services
            .AddDbContext<IdentityDbContext>(_ =>
            {
                _.UseSqlServer(connectionString, _ =>
                {
                    _.MigrationsAssembly(migrationsAssembly);
                    _.UseNodaTime();
                    _.EnableRetryOnFailure(2);
                });
                //o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });


        services
            .AddScoped<IIdentityDbContext, IdentityDbContext>()
            .AddTransient<ITokenService, TokenService>();

        return services;
    }
}