using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Filters
{
    // Concrete implementation resolved by TypeFilterAttribute wrapper below.
    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string? _roles;
        private readonly ILogger<CustomAuthorizationFilter> _logger;

        public CustomAuthorizationFilter(string? roles, ILogger<CustomAuthorizationFilter> logger)
        {
            _roles = roles;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Allow anonymous endpoints to opt-out
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>().Any();
            if (allowAnonymous) return;

            var user = context.HttpContext.User;
            if (user?.Identity is null || !user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized request to {Path}", context.HttpContext.Request.Path);
                context.Result = new ChallengeResult(); // triggers redirect to login via cookie events
                return;
            }

            if (!string.IsNullOrWhiteSpace(_roles))
            {
                var roles = _roles.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim());
                var inRole = roles.Any(r => user.IsInRole(r));
                if (!inRole)
                {
                    _logger.LogWarning("Forbidden - user lacks required roles ({RequiredRoles}) for {Path}", _roles, context.HttpContext.Request.Path);
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
