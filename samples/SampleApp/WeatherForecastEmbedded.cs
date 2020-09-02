using Digipolis.Paging.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SampleApp
{
    public class WeatherForecastEmbedded : IEmbedded<WeatherForecast>
    {
        //When using Newtonsoft.Json:
        [JsonProperty("weatherforecasts")]
        //When using System.Text.Json
        [JsonPropertyName("weatherforecasts")]
        public IEnumerable<WeatherForecast> ResourceList { get; set; }
    }
}
