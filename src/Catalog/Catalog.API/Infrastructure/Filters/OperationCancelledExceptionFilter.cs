using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.API.Infrastructure.Filters
{
    public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<OperationCancelledExceptionFilter> _logger;

        public OperationCancelledExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OperationCancelledExceptionFilter>();
        }
        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is OperationCanceledException)) return;

            _logger.LogInformation($"Request was cancelled");
            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }
    }
}
