using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Filters
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.ActionDescriptor.RouteValues["controller"];
            var action = context.ActionDescriptor.RouteValues["action"];
            _logger.LogInformation("Starting action {Controller}/{Action}", controller, action);

            var sw = Stopwatch.StartNew();
            var executedContext = await next();
            sw.Stop();

            if (executedContext.Exception == null || executedContext.ExceptionHandled)
            {
                _logger.LogInformation("Finished action {Controller}/{Action} in {Elapsed}ms", controller, action, sw.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogError(executedContext.Exception, "Action {Controller}/{Action} threw an exception", controller, action);
            }
        }
    }
}
