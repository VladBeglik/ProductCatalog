using System.IdentityModel.Tokens.Jwt;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Catalog.Application.Infrastructure
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = configuration.GetConnectionString("Identity");


            
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.ClientId = "client-id-of-book-service";
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
                        ValidAudience = "your-client-id"
                    };

                });
            
            
            return services;
        }
    }
}
