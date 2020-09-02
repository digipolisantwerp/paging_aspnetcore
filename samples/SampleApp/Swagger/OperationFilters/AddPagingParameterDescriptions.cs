using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleApp.Swagger.OperationFilters
{
    public class AddPagingParameterDescriptions : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;

            foreach (var parameter in operation.Parameters.Where(x => x.In.HasValue && !String.IsNullOrWhiteSpace(x.In.ToString()) && x.In.ToString().Equals("Query")))
            {
                if (parameter.Name == "sort")
                    parameter.Description = "Sortingield and direction e.g. -id (id descending), id (id ascending)";

                if (parameter.Name == "page")
                    parameter.Description = "Paging - page (default 1)";

                if (parameter.Name == "pagesize")
                    parameter.Description = "Paging - pagesize (default 10)";

                if (parameter.Name == "paging-strategy")
                {
                    parameter.Description = "Available values : WithCount, NoCount (default WithCount)";
                    parameter.Schema = new OpenApiSchema { Type = "string", Enum = new List<IOpenApiAny> { new OpenApiString("WithCount"), new OpenApiString("NoCount") } };
                }

                parameter.Name = parameter.Name.ToLowerInvariant();
            }
        }
    }
}
