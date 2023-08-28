using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.API.Infrastructure.Filters
{
    public class ModelBindingErrorFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            context.Result = new JsonResult
            (
                context.ModelState.Keys.Select(e => new
                {
                    key = e.ToString(),
                    errors = context.ModelState[e]?.Errors.Select(er => er.ErrorMessage)
                })
            );

        }

    }
}
