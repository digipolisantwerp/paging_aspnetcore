using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SampleApp.Swagger.Middleware
{
    public class SwaggerUiRedirectMiddleware
    {
        public SwaggerUiRedirectMiddleware(RequestDelegate next, string url = null)
        {
            NextDelegate = next ?? throw new ArgumentNullException(nameof(next), $"{nameof(next)} cannot be null.");
            Url = String.IsNullOrWhiteSpace(url) ? "swagger/index.html" : url;
        }

        public RequestDelegate NextDelegate { get; private set; }

        public string Url { get; private set; }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/")
            {
                httpContext.Response.Redirect(Url);
                return;
            }

            await NextDelegate(httpContext);
            return;
        }
    }
}
