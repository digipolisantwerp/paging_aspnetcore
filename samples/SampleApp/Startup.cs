using Digipolis.Paging.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SampleApp.Swagger.Extensions;
using SampleApp.Swagger.OperationFilters;

namespace SampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WeatherForecast API",
                    Description = "Dummy API for paging & sorting demo purposes",
                    Version = "v1",
                    License = new OpenApiLicense
                    {
                        Name = "None",
                        Url = null,
                    },
                });

                //paging & sorting specific swagger filters
                options.OperationFilter<AddPagingParameterDescriptions>();
                options.OperationFilter<LowerCaseQueryAndBodyParameters>();
            });

            #endregion

            //register paging and sorting services
            services.AddPaging();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors((policy) =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });

            app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecast API V1"));
            app.UseSwaggerUiRedirect();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
