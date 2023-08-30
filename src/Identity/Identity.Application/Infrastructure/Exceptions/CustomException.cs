namespace Identity.Application.Infrastructure.Exceptions
{
    public class CustomException : Exception, ICustomExceptionMarker
    {
        public CustomException() : base("Server error") { }

        public CustomException(string message) : base(message) { }
    }
}
