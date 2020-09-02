using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Digipolis.Paging.Mapping;

namespace Digipolis.Paging.Startup
{
    public static class DependencyRegistration
    {
        /// <summary>
        /// Extension to register dependencies for the paging library.
        /// </summary>
        /// <param name="services">The given IServiceCollection.</param>
        public static void AddPaging(this IServiceCollection services)
        {
            services.AddTransient<IDataPager, DataPager>();
            services.AddTransient<IPageConverter, PageConverter>();
            services.AddTransient<ILinkGenerator, LinkGenerator>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        }
    }
}
