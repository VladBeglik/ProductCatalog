using System.IdentityModel.Tokens.Jwt;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.SaveToken = true;
                    o.Audience = "Identity";
                    o.Authority = identityUrl;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ClockSkew = new TimeSpan(0, 5, 0)
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = CustomTokenRetriever.FromHeaderAndQueryString(context.Request);
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                    o.Configuration = new OpenIdConnectConfiguration();
                });

            return services;
        }


        #region CustomTokenRetriever

        public class CustomTokenRetriever
        {
            internal const string TokenItemsKey = "idsrv4:tokenvalidation:token";

            static Func<HttpRequest, string> AuthHeaderTokenRetriever { get; set; }
            static Func<HttpRequest, string> QueryStringTokenRetriever { get; set; }

            static CustomTokenRetriever()
            {
                AuthHeaderTokenRetriever = TokenRetrieval.FromAuthorizationHeader();
                QueryStringTokenRetriever = TokenRetrieval.FromQueryString();
            }

            public static string? FromHeaderAndQueryString(HttpRequest request)
            {
                var token = AuthHeaderTokenRetriever(request);

                if (!string.IsNullOrEmpty(token))
                    return token;

                token = QueryStringTokenRetriever(request);

                if (!string.IsNullOrEmpty(token))
                    return token;

                token = request.HttpContext.Items[TokenItemsKey] as string;

                if (!string.IsNullOrEmpty(token))
                    return token;

                return token;
            }
        }
        #endregion

    }
}
