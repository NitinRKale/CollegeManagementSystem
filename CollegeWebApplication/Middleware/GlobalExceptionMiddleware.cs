using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by global middleware for {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // If client expects JSON, return minimal JSON
            var accept = context.Request.Headers["Accept"].ToString();
            if (accept.Contains("application/json") || context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
                return context.Response.WriteAsync(payload);
            }

            // For HTML requests, redirect to error page
            context.Response.Redirect("/Home/Error");
            return Task.CompletedTask;
        }
    }
}