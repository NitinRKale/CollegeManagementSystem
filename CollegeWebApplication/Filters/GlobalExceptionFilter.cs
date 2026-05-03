using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            _logger.LogError(ex, "Unhandled exception in MVC pipeline for {Path}", context.HttpContext.Request.Path);

            // If client expects JSON or the request is an API call, return ProblemDetails JSON
            var accept = context.HttpContext.Request.Headers["Accept"].ToString();
            if (accept.Contains("application/json") || context.HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                var pd = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Detail = ex.Message,
                    Status = 500
                };
                context.Result = new ObjectResult(pd) { StatusCode = 500 };
                context.ExceptionHandled = true;
                return;
            }

            // For regular MVC requests redirect to a friendly error page (Home/Error)
            context.Result = new RedirectToActionResult("Error", "Home", null);
            context.ExceptionHandled = true;
        }
    }
}
