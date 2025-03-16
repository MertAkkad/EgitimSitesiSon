using Microsoft.AspNetCore.Builder;

namespace EgitimSitesi.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
} 