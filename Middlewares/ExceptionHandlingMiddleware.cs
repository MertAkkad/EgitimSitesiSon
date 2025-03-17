using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EgitimSitesi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                
                // Log additional details that might help debugging
                _logger.LogError($"Request path: {context.Request.Path}");
                _logger.LogError($"Request method: {context.Request.Method}");
                
                // If it's an AJAX request, return JSON
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new { error = "An error occurred while processing your request. Please try again later." });
                    await context.Response.WriteAsync(result);
                }
                else
                {
                    // For normal requests, redirect to error page
                    context.Response.Redirect("/Home/Error");
                }
            }
        }
    }
} 