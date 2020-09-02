using Microsoft.AspNetCore.Builder;
using SampleApp.Swagger.Middleware;

namespace SampleApp.Swagger.Extensions
{
    public static class SwaggerExtensions
    {
        public static IApplicationBuilder UseSwaggerUiRedirect(this IApplicationBuilder app, string url = null)
        {
            if (url == null)
                app.UseMiddleware<SwaggerUiRedirectMiddleware>();
            else
                app.UseMiddleware<SwaggerUiRedirectMiddleware>(url);

            return app;
        }
    }
}
