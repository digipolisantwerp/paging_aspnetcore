using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Digipolis.Paging.Predicates;


namespace Digipolis.Paging.UnitTests.Predicates
{
    public class PagePredicatesTests
    {
        public PagePredicatesTests()
        {

        }

        [Fact]
        public void Create_Page_Without_PageNumber_Throws_ArgumentException()
        {
            var queryable = new List<PageClass>
            {
                new PageClass(5)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.CreatePage(null, 1));
        }

        [Fact]
        public void Create_Page_Without_PageSize_Throws_ArgumentException()
        {
            var queryable = new List<PageClass>
            {
                new PageClass(5)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.CreatePage(1, null));
        }

        [Fact]
        public void Create_Page_With_Negative_PageNumber_Throws_ArgumentException()
        {
            var queryable = new List<PageClass>
            {
                new PageClass(5)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.CreatePage(-1, 1));
        }

        [Fact]
        public void Create_Page_With_Negative_PageSize_Throws_ArgumentException()
        {
            var queryable = new List<PageClass>
            {
                new PageClass(5)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.CreatePage(1, -1));
        }

        [Fact]
        public void Create_Page_With_Zero_PageNumber_Throws_ArgumentException()
        {
            var queryable = new List<PageClass>
            {
                new PageClass(5)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.CreatePage(0, 1));
        }

        [Fact]
        public void Create_Page_With_Zero_PageSize_Throws_ArgumentException()
        {
            var queryable = new List<PageClass>
            {
                new PageClass(5)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.CreatePage(1, 0));
        }

        [Fact]
        public void Create_Page_Returns_Pages()
        {
            var queryable = new List<PageClass>
            {
                new PageClass(5),
                new PageClass(7),
                new PageClass(1),
                new PageClass(10),
                new PageClass(4),
                new PageClass(6),
                new PageClass(8)
            }.AsQueryable();

            var result = queryable.CreatePage(1, 3);

            Assert.Equal(3, result.Count());
            Assert.Equal(queryable.First(), result.First());

            result = queryable.CreatePage(2, 3);

            Assert.Equal(3, result.Count());
            Assert.Equal(queryable.Skip(3).Take(1).First(), result.First());

            result = queryable.CreatePage(3, 3);
            Assert.Equal(1, result.Count());
            Assert.Equal(queryable.Skip(6).Take(1).First(), result.First());

            result = queryable.CreatePage(4, 3);
            Assert.Equal(0, result.Count());
        }

        private class PageClass
        {
            public PageClass(int index)
            {
                A = ((char)('a' + index)).ToString();
                B = ((char)('Z' - index)).ToString();
                C = DateTime.Today.AddDays(index);
            }
            public string A { get; set; }

            public string B { get; set; }

            public DateTime? C { get; set; }
        }
    }
}
