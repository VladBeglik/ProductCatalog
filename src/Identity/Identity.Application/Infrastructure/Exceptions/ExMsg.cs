namespace Identity.Application.Infrastructure.Exceptions;

public static class ExMsg
{
    public static class User
    {
        public const string Unauthorized = "Unauthorized";

        public static string AccessDenied() => "Access denied";
        public static string NotFound() => "User is not found";
        public static string UserNotCreated() => "Failed to create user";
        public static string UserNotDeleted() => "Failed to delete user";

    }
}