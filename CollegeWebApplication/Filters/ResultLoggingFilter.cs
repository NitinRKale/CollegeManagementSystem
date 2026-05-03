
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Filters
{
    public class ResultLoggingFilter : IResultFilter
    {
        private readonly ILogger<ResultLoggingFilter> _logger;

        public ResultLoggingFilter(ILogger<ResultLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var ctrl = context.ActionDescriptor.RouteValues["controller"];
            var act = context.ActionDescriptor.RouteValues["action"];
            _logger.LogInformation("Executing result for {Controller}/{Action}", ctrl, act);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            var ctrl = context.ActionDescriptor.RouteValues["controller"];
            var act = context.ActionDescriptor.RouteValues["action"];

            if (context.Exception == null || context.ExceptionHandled)
                _logger.LogInformation("Result executed for {Controller}/{Action}", ctrl, act);
            else
                _logger.LogError(context.Exception, "Result execution failed for {Controller}/{Action}", ctrl, act);
        }
    }
}
