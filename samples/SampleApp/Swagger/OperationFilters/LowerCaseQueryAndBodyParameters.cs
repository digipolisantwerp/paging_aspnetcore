using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace SampleApp.Swagger.OperationFilters
{
    public class LowerCaseQueryAndBodyParameters : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;

            foreach (var parameter in operation.Parameters.Where(x => x.In.HasValue && !String.IsNullOrWhiteSpace(x.In.ToString()) && (x.In.ToString().Equals("Query") || x.In.ToString().Equals("Body"))))
            {
                parameter.Name = parameter.Name.ToLowerInvariant();
            }
        }
    }
}
