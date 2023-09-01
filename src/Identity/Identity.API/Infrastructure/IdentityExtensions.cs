using System.Reflection;
using System.Text;
using Identity.Application.Infrastructure;
using Identity.Domain;
using Identity.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

        
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://localhost:5001";
                options.ClientId = "client-id-of-auth-service";
                options.ClientSecret = "client-secret-of-auth-service";
                options.CallbackPath = "/signin-oidc";
                options.SaveTokens = true; // Сохранять токены для дальнейшего использования
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey("your-signing-key"u8.ToArray()),
                    ValidateIssuer = true,
                    ValidIssuer = "https://localhost:5001",
                    ValidateAudience = true,
                    ValidAudience = "your-client-id",
                };
            });
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