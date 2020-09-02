using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Digipolis.Paging;
using Digipolis.Paging.Mapping;
using Digipolis.Paging.Models;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IDataPager _dataPager;
        private readonly IPageConverter _pageConverter;

        public WeatherForecastController(IDataPager dataPager, IPageConverter pageConverter)
        {
            _dataPager = dataPager ?? throw new ArgumentNullException(nameof(dataPager));
            _pageConverter = pageConverter ?? throw new ArgumentNullException(nameof(pageConverter));
        }

        //example request
        //curl -X GET "http://localhost:7000/WeatherForecast?sort=Summary&page=1&pagesize=15&paging-strategy=WithCount" -H "accept: text/plain"
        /// <summary>
        /// Get weather forecasts paged and filtered
        /// </summary>
        /// <param name="weatherForecastSearchFilter">search filter for weather forecasts</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<WeatherForecast, WeatherForecastEmbedded>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] WeatherForecastSearchFilter weatherForecastSearchFilter)
        {
            #region Business logic & data access layer
            //Typically the following code should be found in the business logic and data access layer, but for demo purposes

            //Construct your query depending on specific filter criteria (using dummy data for example purposes)
            var rng = new Random();
            var query = Enumerable.Range(1, 30).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = _summaries[rng.Next(_summaries.Length)]
            })
            .AsQueryable();

            //Add requested paging and sorting criteria to query and execute
            var dataPage = await _dataPager.Page(query, weatherForecastSearchFilter);

            #endregion

            //Previous method returns a datapage. Convert the response to a model that is compliant with the API requirements
            var pagedResult = _pageConverter.ToPagedResult<WeatherForecast, WeatherForecastEmbedded>(weatherForecastSearchFilter, dataPage);
            return Ok(pagedResult);
        }
    }
}
