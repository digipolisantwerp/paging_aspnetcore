# Paging Toolbox

The Paging toolbox handles both paging and sorting that can be used in ASP.NET Core Web API and Web MVC Projects.

The **SampleApp-project** in the code provides examples of how to implement the toolbox in your project.

## Table of Contents

- [Versions](#versions)
- [Installation](#installation)
- [Dependencies](#dependencies)
- [Configuration in Startup.ConfigureServices](#configuration-in-startupconfigureservices)
  - [Code](#code)
- [Configuration in Startup.Configure](#configuration-in-startupconfigure)
- [Swagger configuration (optional)](#swagger-configuration-(optional)))
- [Usage](#usage)
  - [Execute IQueryable with paging and sorting](#execute-iqueryable-with-paging-and-sorting)
  - [Transforming DataPage<T> to PagedResult<T, TEmbedded>](#transforming-dataPage<t>-to-pagedResult<t,-tembedded>)


## Versions

The latest version of this package targets **.NET Standard 2.0**.

## Installation

To add the toolbox to a project, you add the package to the csproj project file:

```xml
  <ItemGroup>
    <PackageReference Include="Digipolis.Paging" Version="1.0.0" />
  </ItemGroup>
```

or if your project still works with project.json :

``` json
"dependencies": {
    "Digipolis.Paging":  "1.0.0"
 }
```

ALWAYS check the latest version [here](https://github.com/digipolisantwerp/paging_aspnetcore/tree/master/src/Digipolis.Paging/Digipolis.Paging.csproj) before adding the above line !

In Visual Studio you can also use the NuGet Package Manager to do this.

## Dependencies

This toolbox targets **netstandard2.0** and can only be used on a platform implementing this standard (for example .NET Core 2.0). More information about .NET standards can be found [here](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).


## Configuration in Startup.ConfigureServices

The paging framework is registered in the ConfigureServices method of the Startup class.

### Code

Simply call the AddPaging method, no options can be set:

``` csharp
    services.AddPaging();
```

## Configuration in Startup.Configure

The paging toolbox doesn't contain any middleware that has to be added to the pipeline.

## Swagger configuration (optional)

There's an operation filter in the **SampleApp-project** that adds descriptions for paging and sorting parameters to your generated Swagger API documentation when using Swashbuckle NuGet packages.
Source code is available [here](https://github.com/digipolisantwerp/paging_aspnetcore/tree/master/samples/SampleApp/Swagger/OperationFilters/AddPagingParameterDescriptions.cs).

## Usage

It is strongly suggested to inspect the **SampleApp-project** [here](https://github.com/digipolisantwerp/paging_aspnetcore/tree/master/samples/SampleApp/Controllers/WeatherForecastController.cs). Most of the code is self-explanatory so reading the rest of the usage instructions is probably not necessary.


### Execute IQueryable with paging and sorting

Step 1: to add paging to any IQueryable, inject the IDataPager interface in your class first. 

``` csharp
    public class MyClass
    {
      private readonly IDataPager _dataPager;

      public MyClass(IDataPager dataPager)
        {
            _dataPager = dataPager ?? throw new ArgumentNullException(nameof(dataPager));
        }
    }  
```

Step 2: call the async **Page** method. This method requires 2 input parameters: x, which represents an IQueryable, and y, which is an instance of the **PageSortOptions** (supports sorting and paging) or **PageOptions** (only supports paging) class. The method returns an instance of the generic class **DataPage<T>**, which contains the resultset as a list of generic type T. Additionally, this class exposes a few properties containing information regarding paging (page number, page length, total page count and total result count).
Note that this code is executed asynchronously. There is a built-in fail-safe that adds support for any IQueryable<T>, not just IAsyncQueryable<T>.


``` csharp
      public async Task SomeMethod()
        {
            var result = await _dataPager.Page(x, y);
        } 
```

The **PageOptions** class contains 3 properties which are described in the code below. If any of these properties is missing a value, the default value (described in the summary) will be used. If you don't need information about the total number of results, you could slightly improve performance by setting **PagingStrategy** to **PagingStrategy.NoCount**.

``` csharp
        /// <summary>
        /// Page number.
        /// Default value = 1
        /// </summary>
        [JsonProperty("page")]
        [JsonPropertyName("page")]
        [FromQuery(Name = "page")]
        public int Page { get; set; } = Constants.Paging.DefaultPageNumber;

        /// <summary>
        /// Page size.
        /// Default value = 10
        /// </summary>
        [JsonProperty("pagesize")]
        [JsonPropertyName("pagesize")]
        [FromQuery(Name = "pagesize")]
        public int PageSize { get; set; } = Constants.Paging.DefaultPageSize;

        /// <summary>
        /// Option to enable/disable requesting a total result count.
        /// Current values: WithCount or NoCount
        /// Default value = WithCount
        /// </summary>
        [JsonProperty("paging-strategy")]
        [JsonPropertyName("paging-strategy")]
        [FromQuery(Name = "paging-strategy")]
        public PagingStrategy? PagingStrategy { get; set; } = Models.PagingStrategy.WithCount;
  ```
    
  The **PageSortOptions** class extends the previous class with an extra **Sort** property. This property (string) expects a property name or comma seperated list of property names to sort by (case insensitive). If a property name is prefixed with "-", the order of the resultset will reverse (descending). No prefix will result in an ascending order.
  e.g. Sort = "id, -name, date" will primarily order data ascending by id. In case of duplicate id's, these duplicates will be ordered descending by name, and in the unlikely scenario where objects with duplicate ids and names exist, these objects would be ordered ascending by date.
  If this property is not set, the default value will be used. This default value is "id" if the class contains a property named Id. In other cases the first property defind in the class will be used as a fallback.

``` csharp
       /// <summary>
        /// Property name or comma seperated list of the names of the properties to sort by. 
        /// In order to sort descending, set '-' in front of the property name (e.g. "-id"). 
        /// Default value = "id" if an Id property exists. In other cases the first property defind in the class will be used as a fallback.
        /// </summary>
        public string Sort { get; set; } = Sorting.Default;
  ```

### Transforming DataPage<T> to PagedResult<T, TEmbedded>

The DataPage class is very practical to work with, but we wanted to add extra information (described in the Digipolis API requirements) when exposing a paged and sorted set of results through an API. This is where the **PagedResult<T, TEmbedded>** class comes in. 
This type's **Links** property provides dynamic uri's of the first, last, next, previous and current page based on the given paging and sorting criteria, which simplifies the development of standard paging behaviour in an application. There is also a **Page** property that exposes the current page number, page size, the number of total elements and the number of total pages. 
The data itself can be found in a **ResourceList** property which is wrapped in an **Embedded** property. In order to provide a custom JSON property name for the **ResourceList** property, the **PagedResult<T, TEmbedded>** class expects generic type **TEmbedded** which constrains the **IEmbedded<T>** interface. The only requirement to implement this interface is an IEnumerable<T> property named **ResourceList**. Provide this property with a **JsonProperty** attribute (when using Newtonsoft.Json) or a **JsonPropertyName** attribute (when using System.Text.Json) if you need a custom JSON property name for the **ResourceList** property.



Step 1: inject the IPageConverter interface in your class first. 

``` csharp
    public class MyClass
    {
      private readonly IPageConverter _pageConverter;

      public MyClass(IPageConverter pageConverter)
        {
            _pageConverter = pageConverter ?? throw new ArgumentNullException(nameof(pageConverter));
        }
    }  
```


Step 2: create TEmbedded class. 

``` csharp
    public class WeatherForecastEmbedded : IEmbedded<WeatherForecast>
    {
        //When using Newtonsoft.Json:
        [JsonProperty("weatherforecasts")]
        //When using System.Text.Json
        [JsonPropertyName("weatherforecasts")]
        public IEnumerable<WeatherForecast> ResourceList { get; set; }
    }
```


Step 3: execute the converter's ToPagedResult method. Parameter searchFilter can be of type **PageOptions** or **PageSortOptions** and dataPage expects **DataPage<T>** (or DataPage<WeatherForecast> in this example). The classes are already described in [Execute IQueryable with paging and sorting](#execute-iqueryable-with-paging-and-sorting)

``` csharp
      public PagedResult<WeatherForecast, WeatherForecastEmbedded> SomeMethod(PageSortOptions searchFilter, DataPage<WeatherForecast> dataPage)
        {
            var pagedResult = _pageConverter.ToPagedResult<WeatherForecast, WeatherForecastEmbedded>(searchFilter, dataPage);
            return pagedResult;
        } 
```

Step 4 (optional): expose this result through your Web API

``` csharp
      public async Task<IActionResult> SomeApiCall(PageSortOptions searchFilter)
        {
            var pagedResult = _pageConverter.ToPagedResult<WeatherForecast, WeatherForecastEmbedded>(searchFilter, dataPage);
            return Ok(pagedResult);
        } 
```

Example of a PagedResult JSON response, in this case using "weatherforecasts" as a custom name for the **ResourceList** property
 ``` json
{
  "_links": {
    "first": {
      "href": "string"
    },
    "prev": {
      "href": "string"
    },
    "self": {
      "href": "string"
    },
    "next": {
      "href": "string"
    },
    "last": {
      "href": "string"
    }
  },
  "_embedded": {
    "weatherforecasts": [
      {
        "date": "2020-01-01T00:00:00.000Z",
        "temperatureC": 0,
        "temperatureF": 0,
        "summary": "string"
      }
    ]
  },
  "_page": {
    "number": 0,
    "size": 0,
    "totalElements": 0,
    "totalPages": 0
  }
}
```
