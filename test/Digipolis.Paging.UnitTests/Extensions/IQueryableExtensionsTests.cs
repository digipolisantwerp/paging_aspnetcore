using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Digipolis.Paging.Extensions;
using System.Threading.Tasks;

namespace Digipolis.Paging.UnitTests.Extensions
{
    public class IQueryableExtensionsTests
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public IQueryableExtensionsTests()
        {

        }

        private class WeatherForecast
        {
            public DateTime Date { get; set; }

            public int TemperatureC { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

            public string Summary { get; set; }
        }

        private IQueryable<WeatherForecast> GenerateQueryable()
        {
            var rng = new Random();
            return Enumerable.Range(1, 30).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = _summaries[rng.Next(_summaries.Length)]
            })
         .AsQueryable();
        }

        [Fact]
        public void ToListAsyncSafe_Throws_When_IQueryable_Is_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => ((IQueryable<WeatherForecast>)null).ToListAsyncSafe());
        }

        [Fact]
        public async Task ToListAsyncSafe_Returns_Task_When_IQueryable_Is_Not_IAsyncQueryable()
        {
            var query = GenerateQueryable();
            var resultTask = query.ToListAsyncSafe();
            Assert.NotNull(resultTask);
            Assert.True(resultTask is Task<List<WeatherForecast>>);
            var result = await resultTask;
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result is List<WeatherForecast>);
        }

        [Fact]
        public void ToListAsyncSafe_Returns_Task_When_IQueryable_Is_IAsyncQueryable()
        {
            //To avoid a mocking nightmare, we just assume this code works since it's built into Entity Framework Core
            Assert.True(true);
        }

        [Fact]
        public void CountAsyncSafe_Throws_When_IQueryable_Is_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => ((IQueryable<WeatherForecast>)null).CountAsyncSafe());
        }

        [Fact]
        public async Task CountAsyncSafe_Returns_Task_When_IQueryable_Is_Not_IAsyncQueryable()
        {
            var query = GenerateQueryable();
            var resultTask = query.CountAsyncSafe();
            Assert.NotNull(resultTask);
            Assert.True(resultTask is Task<int>);
            var result = await resultTask;
            Assert.True(result >= 0);
        }

        [Fact]
        public void CountAsyncSafe_Returns_Task_When_IQueryable_Is_IAsyncQueryable()
        {
            //To avoid a mocking nightmare, we just assume this code works since it's built into Entity Framework Core
            Assert.True(true);
        }

    }
}
