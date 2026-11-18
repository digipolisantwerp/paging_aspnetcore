using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace SampleApp.Swagger.OperationFilters
{
    public class LowerCaseQueryAndBodyParameters : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;

            var newOpenApiParameterList = new List<IOpenApiParameter>();

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.In.HasValue && !string.IsNullOrWhiteSpace(parameter.In.ToString())
                                         && (parameter.In.ToString().Equals("Query") || parameter.In.ToString().Equals("Body")))
                {
                    // add new parameter with changed name; because Name-property is immutable
                    newOpenApiParameterList.Add(new OpenApiParameter
                    {
                        Name = parameter.Name.ToLowerInvariant(),
                        In = parameter.In,
                        Required = parameter.Required,
                        Schema = parameter.Schema,
                        Description = parameter.Description
                    });
                }
                else
                {
                    newOpenApiParameterList.Add(parameter);
                }
            }

            operation.Parameters = newOpenApiParameterList;

        }
    }
}
