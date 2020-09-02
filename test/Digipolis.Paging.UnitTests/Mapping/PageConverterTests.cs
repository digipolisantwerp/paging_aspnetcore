using Digipolis.Paging.Mapping;
using Digipolis.Paging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Xunit;

namespace Digipolis.Paging.UnitTests.Mapping
{
    public class PageConverterTests
    {
        private static readonly string _host = "host.com";
        private static readonly string _port = "100";
        private static readonly string _path = "path";

        private static readonly string _href = $"{_host}:{_port}/{_path}?query=somequery";

        private static readonly int _page = 2;
        private static readonly int _pagesize = 20;
        private static readonly int _totalCount = 1000;
        private static readonly int _totalPages = (int)Math.Ceiling((double)_totalCount / _pagesize);

        private static readonly string _sort = "-id";

        public PageConverterTests()
        {

        }

        private PageConverter SetupPageConverter()
        {
            var ctx = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ActionName = "a",
                    ControllerName = "b"
                },
                RouteData = new Microsoft.AspNetCore.Routing.RouteData()
            };
            ctx.HttpContext.Request.Host = new HostString($"{_host}:{_port}");
            ctx.HttpContext.Request.Path = $"/{_path}";

            var actionContextAccessor = new ActionContextAccessor
            {
                ActionContext = ctx
            };

            var linkGeneratorMock = new Mock<ILinkGenerator>();
            linkGeneratorMock.Setup(x => x.GenerateLink(It.IsAny<PageOptions>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>())).Returns(new Link { Href = _href });
            linkGeneratorMock.Setup(x => x.GenerateLink(It.IsAny<PageSortOptions>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>())).Returns(new Link { Href = _href });
            linkGeneratorMock.Setup(x => x.GenerateLink(It.IsAny<PageOptions>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns(new Link { Href = _href });
            linkGeneratorMock.Setup(x => x.GenerateLink(It.IsAny<PageSortOptions>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns(new Link { Href = _href });

            return new PageConverter(actionContextAccessor, linkGeneratorMock.Object);

        }

        private class DummyObject
        {
            public int DummyProperty { get; set; }
        }

        private IEnumerable<DummyObject> GenerateEnumerable(int items)
        {
            var list = new List<DummyObject>();
            for (int i = 0; i < items; i++)
            {
                list.Add(new DummyObject { DummyProperty = i });
            }
            return list.AsEnumerable<DummyObject>();
        }

        private class DummyObjectEmbedded : IEmbedded<DummyObject>
        {
            [JsonProperty("dummyobjects")]
            [JsonPropertyName("dummyobjects")]
            public IEnumerable<DummyObject> ResourceList { get; set; }
        }

        [Fact]
        public void ToPagedResult_Returns_PagedResult()
        {
            var pageConverter = SetupPageConverter();

            var dummyObjects = GenerateEnumerable(_pagesize);

            var result = pageConverter.ToPagedResult<DummyObject, DummyObjectEmbedded>(
                pageOptions: new PageOptions
                {
                    Page = _page,
                    PageSize = _pagesize
                },
                data: dummyObjects,
                total: _totalCount);

            Assert.NotNull(result);

            //Links are being mocked, underlying code is tested by LinkGeneratorTests.cs
            Assert.NotNull(result.Links);
            Assert.Equal(_href, result.Links.First.Href);
            Assert.Equal(_href, result.Links.Last.Href);
            Assert.Equal(_href, result.Links.Self.Href);
            Assert.Equal(_href, result.Links.Next.Href);
            Assert.Equal(_href, result.Links.Previous.Href);

            Assert.NotNull(result.Page);
            Assert.Equal(_page, result.Page.Number);
            Assert.Equal(_pagesize, result.Page.Size);
            Assert.Equal(_totalCount, result.Page.TotalElements);
            Assert.Equal(_totalPages, result.Page.TotalPages);

            Assert.NotNull(result.Embedded);
            Assert.NotNull(result.Embedded.ResourceList);
            Assert.Contains(dummyObjects.First().DummyProperty, result.Embedded.ResourceList.Select(x => x.DummyProperty));
        }

        [Fact]
        public void ToPagedResult_Without_Data_Returns_PagedResult()
        {
            var pageConverter = SetupPageConverter();

            var result = pageConverter.ToPagedResult<DummyObject, DummyObjectEmbedded>(
                pageOptions: new PageOptions
                {
                    Page = _page,
                    PageSize = _pagesize
                },
                data: null,
                total: _pagesize);

            Assert.NotNull(result);

            //Links are being mocked, underlying code is tested by LinkGeneratorTests.cs
            Assert.NotNull(result.Links);
            Assert.Equal(_href, result.Links.First.Href);
            Assert.Equal(_href, result.Links.Last.Href);
            Assert.Equal(_href, result.Links.Self.Href);
            //Only one page => no next results
            Assert.Null(result.Links.Next);
            Assert.Equal(_href, result.Links.Previous.Href);

            Assert.NotNull(result.Page);
            Assert.Equal(_page, result.Page.Number);
            //Getting page 2 => only the previous page contains items
            Assert.Equal(0, result.Page.Size);
            Assert.Equal(_pagesize, result.Page.TotalElements);
            Assert.Equal(1, result.Page.TotalPages);

            Assert.NotNull(result.Embedded);
            Assert.NotNull(result.Embedded.ResourceList);
            Assert.Empty(result.Embedded.ResourceList);
        }

        [Fact]
        public void ToPagedResult_Without_PageSize_Returns_PagedResult()
        {
            var pageConverter = SetupPageConverter();

            var result = pageConverter.ToPagedResult<DummyObject, DummyObjectEmbedded>(
                pageOptions: new PageOptions
                {
                    Page = _page,
                    PageSize = 0
                },
                data: new List<DummyObject>(),
                total: _totalCount);

            Assert.NotNull(result);

            //Links are being mocked, underlying code is tested by LinkGeneratorTests.cs
            Assert.NotNull(result.Links);
            Assert.Equal(_href, result.Links.First.Href);
            Assert.Equal(_href, result.Links.Last.Href);
            Assert.Equal(_href, result.Links.Self.Href);
            //Empty pagesize => unable to determine next page
            Assert.Null(result.Links.Next);
            Assert.Equal(_href, result.Links.Previous.Href);

            Assert.NotNull(result.Page);
            Assert.Equal(_page, result.Page.Number);
            Assert.Equal(0, result.Page.Size);
            Assert.Equal(_totalCount, result.Page.TotalElements);
            Assert.Equal(0, result.Page.TotalPages);

            Assert.NotNull(result.Embedded);
            Assert.NotNull(result.Embedded.ResourceList);
            Assert.Empty(result.Embedded.ResourceList);
        }

        [Fact]
        public void ToPagedResult_With_Sorting_Returns_PagedResult()
        {
            var pageConverter = SetupPageConverter();

            var dummyObjects = GenerateEnumerable(_pagesize);

            var result = pageConverter.ToPagedResult<DummyObject, DummyObjectEmbedded>(
                pageSortOptions: new PageSortOptions
                {
                    Page = _page,
                    PageSize = _pagesize,
                    Sort = _sort
                },
                data: dummyObjects,
                total: _totalCount);

            Assert.NotNull(result);

            //Links are being mocked, underlying code is tested by LinkGeneratorTests.cs
            Assert.NotNull(result.Links);
            Assert.Equal(_href, result.Links.First.Href);
            Assert.Equal(_href, result.Links.Last.Href);
            Assert.Equal(_href, result.Links.Self.Href);
            Assert.Equal(_href, result.Links.Next.Href);
            Assert.Equal(_href, result.Links.Previous.Href);

            Assert.NotNull(result.Page);
            Assert.Equal(_page, result.Page.Number);
            Assert.Equal(_pagesize, result.Page.Size);
            Assert.Equal(_totalCount, result.Page.TotalElements);
            Assert.Equal(_totalPages, result.Page.TotalPages);

            Assert.NotNull(result.Embedded);
            Assert.NotNull(result.Embedded.ResourceList);
            Assert.Contains(dummyObjects.First().DummyProperty, result.Embedded.ResourceList.Select(x => x.DummyProperty));
        }

        [Fact]
        public void ToPagedResult_DataPage_Returns_PagedResult()
        {
            var pageConverter = SetupPageConverter();

            var dummyObjects = GenerateEnumerable(_pagesize);

            var result = pageConverter.ToPagedResult<DummyObject, DummyObjectEmbedded>(
                pageOptions: new PageOptions
                {
                    Page = _page,
                    PageSize = _pagesize
                },
                new DataPage<DummyObject>(dummyObjects.ToList(), _totalCount)
                );

            Assert.NotNull(result);

            //Links are being mocked, underlying code is tested by LinkGeneratorTests.cs
            Assert.NotNull(result.Links);
            Assert.Equal(_href, result.Links.First.Href);
            Assert.Equal(_href, result.Links.Last.Href);
            Assert.Equal(_href, result.Links.Self.Href);
            Assert.Equal(_href, result.Links.Next.Href);
            Assert.Equal(_href, result.Links.Previous.Href);

            Assert.NotNull(result.Page);
            Assert.Equal(_page, result.Page.Number);
            Assert.Equal(_pagesize, result.Page.Size);
            Assert.Equal(_totalCount, result.Page.TotalElements);
            Assert.Equal(_totalPages, result.Page.TotalPages);

            Assert.NotNull(result.Embedded);
            Assert.NotNull(result.Embedded.ResourceList);
            Assert.Contains(dummyObjects.First().DummyProperty, result.Embedded.ResourceList.Select(x => x.DummyProperty));
        }

        [Fact]
        public void ToPagedResult_DataPage_With_Sorting_Returns_PagedResult()
        {
            var pageConverter = SetupPageConverter();

            var dummyObjects = GenerateEnumerable(_pagesize);

            var result = pageConverter.ToPagedResult<DummyObject, DummyObjectEmbedded>(
                pageSortOptions: new PageSortOptions
                {
                    Page = _page,
                    PageSize = _pagesize,
                    Sort = _sort
                },
                new DataPage<DummyObject>(dummyObjects.ToList(),_totalCount)
                );

            Assert.NotNull(result);

            //Links are being mocked, underlying code is tested by LinkGeneratorTests.cs
            Assert.NotNull(result.Links);
            Assert.Equal(_href, result.Links.First.Href);
            Assert.Equal(_href, result.Links.Last.Href);
            Assert.Equal(_href, result.Links.Self.Href);
            Assert.Equal(_href, result.Links.Next.Href);
            Assert.Equal(_href, result.Links.Previous.Href);

            Assert.NotNull(result.Page);
            Assert.Equal(_page, result.Page.Number);
            Assert.Equal(_pagesize, result.Page.Size);
            Assert.Equal(_totalCount, result.Page.TotalElements);
            Assert.Equal(_totalPages, result.Page.TotalPages);

            Assert.NotNull(result.Embedded);
            Assert.NotNull(result.Embedded.ResourceList);
            Assert.Contains(dummyObjects.First().DummyProperty, result.Embedded.ResourceList.Select(x => x.DummyProperty));
        }
    }
}
