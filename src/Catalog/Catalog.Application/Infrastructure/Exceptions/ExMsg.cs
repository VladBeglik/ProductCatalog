namespace Catalog.Application.Infrastructure.Exceptions;

public static class ExMsg
{
    public static class Book
    {
        public static string NotFound() => "Book not found";
    }  
    public static class Order
    {
        public static string NotFound() => "Order not found";
    }
}