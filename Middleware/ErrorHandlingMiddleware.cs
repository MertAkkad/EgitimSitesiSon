using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EgitimSitesi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception in application: {Message}", ex.Message);
                
                // Log detailed information for database errors
                if (ex is DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
                }
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = JsonSerializer.Serialize(new
            {
                error = "An error occurred while processing your request.",
                detail = exception.Message
            });

            return context.Response.WriteAsync(response);
        }
    }
} 