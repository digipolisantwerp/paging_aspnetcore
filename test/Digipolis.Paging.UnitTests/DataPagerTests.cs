using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Digipolis.Paging.UnitTests
{
    public class DataPagerTests
    {
        private static readonly int _page = 3;
        private static readonly int _pagesize = 12;
        private static readonly int _totalCount = 1000;
        private static readonly int _totalPages = (int)Math.Ceiling((double)_totalCount / _pagesize);

        public DataPagerTests()
        {

        }

        private class DummyObject
        {
            public int DummyProperty { get; set; }
        }

        private IQueryable<DummyObject> GenerateEnumerable(int items)
        {
            var list = new List<DummyObject>();
            for (int i = 0; i < items; i++)
            {
                list.Add(new DummyObject { DummyProperty = i });
            }
            return list.AsQueryable();
        }

        [Fact]
        public async Task Page_And_Sort_With_Null_PagingStrategy_Returns_TotalCount()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageSortOptions
            {
                Page=_page,
                PageSize=_pagesize,
                PagingStrategy = null,
                Sort = "DummyProperty"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(_totalCount, result.TotalEntityCount);
            Assert.Equal(_totalPages, result.TotalPageCount);
            Assert.Equal(_page, result.PageNumber);
            Assert.Equal(_pagesize, result.PageLength);
        }

        [Fact]
        public async Task Page_And_Sort_With_WithCount_PagingStrategy_Returns_TotalCount()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount,
                Sort = "DummyProperty"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(_totalCount, result.TotalEntityCount);
            Assert.Equal(_totalPages, result.TotalPageCount);
            Assert.Equal(_page, result.PageNumber);
            Assert.Equal(_pagesize, result.PageLength);
        }

        [Fact]
        public async Task Page_And_Sort_With_NoCount_PagingStrategy_Returns_TotalCount_Zero()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.NoCount,
                Sort = "DummyProperty"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(0, result.TotalEntityCount);
            Assert.Equal(0, result.TotalPageCount);
            Assert.Equal(_page, result.PageNumber);
            Assert.Equal(_pagesize, result.PageLength);
        }

        [Fact]
        public async Task Page_And_Sort_Returns_Number_Of_Items_Equal_To_PageSize()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount,
                Sort = "DummyProperty"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(_pagesize, result.Data.Count);
        }

        [Fact]
        public async Task Page_And_Sort_Last_Page_Returns_Number_Of_Items_Smaller_Than_PageSize()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageSortOptions
            {
                Page = _totalPages,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount,
                Sort = "DummyProperty"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.True(_pagesize > result.Data.Count);
        }

        [Fact]
        public async Task Page_And_Sort_Returns_List_Of_Data()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount,
                Sort = "DummyProperty"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(typeof(List<DummyObject>), result.Data.GetType());
        }

        [Fact]
        public async Task Page_And_Sort_Returns_List_Of_Data_Sorted()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageSortOptions
            {
                Page = 1,
                PageSize = _totalCount,
                PagingStrategy = Models.PagingStrategy.WithCount,
                Sort = "DummyProperty"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(typeof(List<DummyObject>), result.Data.GetType());
            for (int i = 0; i < _totalCount; i++)
            {
                Assert.Equal(i, result.Data.Skip(i).Take(1).First().DummyProperty);
            }
        }


        [Fact]
        public async Task Page_With_Null_PagingStrategy_Returns_TotalCount()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = null
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(_totalCount, result.TotalEntityCount);
            Assert.Equal(_totalPages, result.TotalPageCount);
            Assert.Equal(_page, result.PageNumber);
            Assert.Equal(_pagesize, result.PageLength);
        }

        [Fact]
        public async Task Page_With_WithCount_PagingStrategy_Returns_TotalCount()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(_totalCount, result.TotalEntityCount);
            Assert.Equal(_totalPages, result.TotalPageCount);
            Assert.Equal(_page, result.PageNumber);
            Assert.Equal(_pagesize, result.PageLength);
        }

        [Fact]
        public async Task Page_With_NoCount_PagingStrategy_Returns_TotalCount_Zero()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.NoCount
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(0, result.TotalEntityCount);
            Assert.Equal(0, result.TotalPageCount);
            Assert.Equal(_page, result.PageNumber);
            Assert.Equal(_pagesize, result.PageLength);
        }

        [Fact]
        public async Task Page_Returns_Number_Of_Items_Equal_To_PageSize()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(_pagesize, result.Data.Count);
        }

        [Fact]
        public async Task Page_Last_Page_Returns_Number_Of_Items_Smaller_Than_PageSize()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageOptions
            {
                Page = _totalPages,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.True(_pagesize > result.Data.Count);
        }

        [Fact]
        public async Task Page__Returns_List_Of_Data()
        {
            var result = await new DataPager().Page(GenerateEnumerable(_totalCount), new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = Models.PagingStrategy.WithCount
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(typeof(List<DummyObject>), result.Data.GetType());
        }
    }
}
