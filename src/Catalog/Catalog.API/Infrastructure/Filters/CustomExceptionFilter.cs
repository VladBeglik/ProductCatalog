using System.Net;
using Catalog.Application.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Catalog.API.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        private const string CONTENT_TYPE = "application/json";

        public CustomExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomExceptionFilter>();
        }

        public override void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case CustomValidationException exception:
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    context.Result = new JsonResult(new
                    {
                        errors = exception.Failures
                    });

                    return;
                case ICustomExceptionMarker _:
                    {
                        context.HttpContext.Response.Headers.Add("X-Be-Error", "1");

                        context.HttpContext.Response.ContentType = CONTENT_TYPE;

                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;


                        switch (context.Exception)
                        {
                            case CustomException _:
                                context.Result = new JsonResult(new { error = context.Exception.Message });
                                return;
                        }
                        return;
                    }
            }

            var code = HttpStatusCode.InternalServerError;

            _logger.LogError(context.Exception, context.Exception.Message);

            context.HttpContext.Response.ContentType = CONTENT_TYPE;

            context.HttpContext.Response.StatusCode = (int)code;
            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                stackTrace = context.Exception.StackTrace
            });
        }
    }
}
