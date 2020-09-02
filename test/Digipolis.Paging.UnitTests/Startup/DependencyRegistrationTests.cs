using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Digipolis.Paging.Mapping;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Digipolis.Paging.UnitTests.Startup
{
    public class DependencyRegistrationTests
    {
        public DependencyRegistrationTests()
        {
        }

        [Fact]
        public void Add_Paging_Should_Resolve_DataPager()
        {
            var services = new ServiceCollection();

            services.AddTransient<IDataPager, DataPager>();

            var serviceProvider = services.BuildServiceProvider();

            var matchService = serviceProvider.GetService<IDataPager>();

            Assert.NotNull(matchService);
        }

        [Fact]
        public void Add_Paging_Should_Resolve_ActionContextAccessor()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            var serviceProvider = services.BuildServiceProvider();

            var matchService = serviceProvider.GetService<IActionContextAccessor>();

            Assert.NotNull(matchService);
        }

        [Fact]
        public void Add_Paging_Should_Resolve_UrlHelperFactory()
        {
            var services = new ServiceCollection();

            services.AddScoped<IUrlHelperFactory, UrlHelperFactory>();

            var serviceProvider = services.BuildServiceProvider();

            var matchService = serviceProvider.GetService<IUrlHelperFactory>();

            Assert.NotNull(matchService);
        }

        [Fact]
        public void Add_Paging_Should_Resolve_LinkGenerator()
        {
            var services = new ServiceCollection();

            services.AddTransient<ILinkGenerator, LinkGenerator>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelperFactory, UrlHelperFactory>();

            var serviceProvider = services.BuildServiceProvider();

            var matchService = serviceProvider.GetService<ILinkGenerator>();

            Assert.NotNull(matchService);
        }

        [Fact]
        public void Add_Paging_Should_Resolve_PageConverter()
        {
            var services = new ServiceCollection();

            services.AddTransient<IPageConverter, PageConverter>();
            services.AddTransient<ILinkGenerator, LinkGenerator>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelperFactory, UrlHelperFactory>();

            var serviceProvider = services.BuildServiceProvider();

            var matchService = serviceProvider.GetService<IPageConverter>();

            Assert.NotNull(matchService);
        }
    }
}
