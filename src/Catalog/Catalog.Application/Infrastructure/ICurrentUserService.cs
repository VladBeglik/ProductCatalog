namespace Catalog.Application.Infrastructure;

public interface ICurrentUserService
{
    string UserId { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(string role);

    bool IsContainsPart(string partOfRole);

    string GetClaimValue(string claimKey);
}