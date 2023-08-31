using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Catalog.Application.Infrastructure;
using Identity.Application.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Catalog.API.Infrastructure
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            InitializeUser();

            var n = _httpContextAccessor.HttpContext.User.Identity.Name;
        }

        private void InitializeUser()
        {
            var token = GetAuthorizationToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                IsAuthenticated = false;
                return;
            }

            var userId = GetUserIdFromToken(token);
            if (string.IsNullOrWhiteSpace(userId))
            {
                IsAuthenticated = false;
                return;
            }

            UserId = userId;
            IsAuthenticated = true;
        }

        private string GetAuthorizationToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var authorizationHeader = httpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                return null;

            return authorizationHeader.Substring("Bearer ".Length);
        }

        private string GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                return userId;
            }
            catch (Exception exception)
            {
                throw new CustomException();
            }
        }

        public string UserId { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public bool IsInRole(string role)
        {
            if (!IsAuthenticated)
                return false;

            return _httpContextAccessor.HttpContext.User.IsInRole(role);
        }

        public bool IsContainsPart(string partOfRole)
        {
            if (!IsAuthenticated)
                return false;

            return _httpContextAccessor.HttpContext.User.Claims?.Any(claim =>
                claim.Type == "role" && claim.Value.Contains(partOfRole)) ?? false;
        }

        public string GetClaimValue(string claimKey)
        {
            return _httpContextAccessor?.HttpContext?.User?
                .FindFirstValue(claimKey);
        }
    }
}