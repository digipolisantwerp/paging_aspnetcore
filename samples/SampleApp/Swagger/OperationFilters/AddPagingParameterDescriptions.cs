using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SampleApp.Swagger.OperationFilters
{
    public class AddPagingParameterDescriptions : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;

            var newOpenApiParameterList = new List<IOpenApiParameter>();

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.In.HasValue && !string.IsNullOrWhiteSpace(parameter.In.ToString())
                                         && parameter.In.ToString().Equals("Query"))
                {
                    var parameterDescription = parameter.Description;
                    var parameterSchema = parameter.Schema;

                    if (parameter.Name == "sort")
                        parameterDescription = "Sortingield and direction e.g. -id (id descending), id (id ascending)";

                    if (parameter.Name == "page")
                        parameterDescription = "Paging - page (default 1)";

                    if (parameter.Name == "pagesize")
                        parameterDescription = "Paging - pagesize (default 10)";

                    if (parameter.Name == "paging-strategy")
                    {
                        parameterDescription = "Available values : WithCount, NoCount (default WithCount)";

                        var enumCountOptions = new List<JsonNode> { JsonValue.Create("WithCount"), JsonValue.Create("NoCount") };
                        parameterSchema = new OpenApiSchema { Type = JsonSchemaType.String, Enum = enumCountOptions };
                    }

                    var newParameter = new OpenApiParameter()
                    {
                        Name = parameter.Name.ToLowerInvariant(),
                        In = parameter.In,
                        Required = parameter.Required,
                        Schema = parameterSchema,
                        Description = parameterDescription
                    };

                    // add new parameter with changed name; because Name-property is immutable
                    newOpenApiParameterList.Add(newParameter);
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
