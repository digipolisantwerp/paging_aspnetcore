using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Digipolis.Paging.Constants;
using Digipolis.Paging.Models;
using Digipolis.Paging.Mapping;

namespace Digipolis.Paging.UnitTests.Links
{
    public class LinkGeneratorTests
    {
        private static readonly string _host = "host.com";
        private static readonly string _port = "100";
        private static readonly string _path = "path";

        private static readonly int _page = 2;
        private static readonly int _pagesize = 3;
        private static readonly string _sort = "-id";
        private static readonly PagingStrategy _pagingstrategy = PagingStrategy.NoCount;
        public LinkGeneratorTests()
        {

        }

        private string GetQuery(bool usePage = true, bool usePageSize = true, bool useSort = true, bool usePagingStrategy = true, bool useIncorrectSyntaxPagingStrategy = false)
        {
            var query = $"?query=somequery";
            if (usePage)
                query = $"{query}&page={_page}";
            if (usePageSize)
                query = $"{query}&pagesize ={_pagesize}";
            if (useSort)
                query = $"{query}&sort ={ _sort}";
            if (usePagingStrategy)
                query = $"{query}&paging-strategy={_pagingstrategy.ToString().ToLowerInvariant()}";
            if (useIncorrectSyntaxPagingStrategy)
                query = $"{query}&pagingstrategy={_pagingstrategy.ToString().ToLowerInvariant()}";
            return query;
        }

        private string GetRelativeUri(bool usePage = true, bool usePageSize = true, bool useSort = true, bool usePagingStrategy = true, bool useIncorrectSyntaxPagingStrategy = false)
        {
            return $"/{_path}{GetQuery(usePage, usePageSize, useSort, usePagingStrategy, useIncorrectSyntaxPagingStrategy)}";
        }

        private string GetFullUri(bool usePage = true, bool usePageSize = true, bool useSort = true, bool usePagingStrategy = true, bool useIncorrectSyntaxPagingStrategy = false)
        {
            return $"{_host}:{_port}/{_path}{GetQuery(usePage, usePageSize, useSort, usePagingStrategy, useIncorrectSyntaxPagingStrategy)}";
        }

        private LinkGenerator SetupLinkGenerator(bool usePort = true, bool usePage = true, bool usePageSize = true, bool useSort = true, bool usePagingStrategy = true, bool useIncorrectSyntaxPagingStrategy = false)
        {
            var ctx = new ActionContext
            {
                HttpContext = new DefaultHttpContext()
            };
            ctx.HttpContext.Request.Host = usePort ? new HostString($"{_host}:{_port}") : new HostString($"{_host}");
            ctx.HttpContext.Request.Path = $"/{_path}";

            var actionContextAccessor = new ActionContextAccessor
            {
                ActionContext = ctx
            };

            var urlHelperMock = new Mock<IUrlHelper>();



            urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(GetRelativeUri(usePage, usePageSize, useSort, usePagingStrategy, useIncorrectSyntaxPagingStrategy));
            urlHelperMock.Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>())).Returns(GetRelativeUri(usePage, usePageSize, useSort, usePagingStrategy, useIncorrectSyntaxPagingStrategy));

            var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
            urlHelperFactoryMock.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelperMock.Object);

            var linkGenerator = new LinkGenerator(actionContextAccessor, urlHelperFactoryMock.Object);

            return linkGenerator;
        }

        [Fact]
        public void GetAbsoluteUrlBuilder_Returns_UriBuilder_Without_Path()
        {
            var linkGenerator = SetupLinkGenerator();

            var uriBuilder = linkGenerator.GetAbsoluteUrlBuilder();

            Assert.NotNull(uriBuilder);
            Assert.Equal(_host, uriBuilder.Host);
            Assert.Equal(_port, uriBuilder.Port.ToString());
            Assert.NotEqual(_path, uriBuilder.Path);
            Assert.Equal("/", uriBuilder.Path);
        }

        [Fact]
        public void GetAbsoluteUrlBuilder_Without_Port_Returns_UriBuilder_Without_Path_And_Port()
        {
            var linkGenerator = SetupLinkGenerator(false);

            var uriBuilder = linkGenerator.GetAbsoluteUrlBuilder();

            Assert.NotNull(uriBuilder);
            Assert.Equal(_host, uriBuilder.Host);
            Assert.Equal(-1, uriBuilder.Port);
            Assert.NotEqual(_path, uriBuilder.Path);
            Assert.Equal("/", uriBuilder.Path);
        }

        [Fact]
        public void GetFullUrlBuilder_Returns_UriBuilder_With_Path()
        {
            var linkGenerator = SetupLinkGenerator();

            var uriBuilder = linkGenerator.GetFullUrlBuilder($"/{_path}");

            Assert.NotNull(uriBuilder);
            Assert.Equal(_host, uriBuilder.Host);
            Assert.Equal(_port, uriBuilder.Port.ToString());
            Assert.Equal($"/{_path}", uriBuilder.Path);
            Assert.Empty(uriBuilder.Query);
        }

        [Fact]
        public void GetFullUrlBuilder_Returns_UriBuilder_With_Path_And_Query()
        {
            var linkGenerator = SetupLinkGenerator();

            var uriBuilder = linkGenerator.GetFullUrlBuilder(GetRelativeUri());

            Assert.NotNull(uriBuilder);
            Assert.Equal(_host, uriBuilder.Host);
            Assert.Equal(_port, uriBuilder.Port.ToString());
            Assert.Equal($"/{_path}", uriBuilder.Path);
            Assert.Equal(GetQuery(), uriBuilder.Query);
        }

        [Fact]
        public void AbsoluteRoute_Returns_Full_Uri()
        {
            var linkGenerator = SetupLinkGenerator();

            var uriString = linkGenerator.AbsoluteRoute("a", null);

            Assert.NotNull(uriString);
            Assert.NotEmpty(uriString);
            Assert.Equal(GetFullUri(), uriString);
        }

        [Fact]
        public void AbsoluteAction_Returns_Full_Uri()
        {
            var linkGenerator = SetupLinkGenerator();

            var uriString = linkGenerator.AbsoluteAction("a", "b", null);

            Assert.NotNull(uriString);
            Assert.NotEmpty(uriString);
            Assert.Equal(GetFullUri(), uriString);
        }

        [Fact]
        public void GenerateRouteValues_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator();

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy,
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(_pagingstrategy, routeValueDictionary[QueryParam.PagingDashStrategy]);
        }

        [Fact]
        public void GenerateRouteValues_Without_Page_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator(usePage: false);

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new Models.PageOptions
            {
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy,
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(_pagingstrategy, routeValueDictionary[QueryParam.PagingDashStrategy]);
        }

        [Fact]
        public void GenerateRouteValues_Without_PageSize_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator(usePageSize: false);

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new Models.PageOptions
            {
                Page = _page,
                PagingStrategy = _pagingstrategy,
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(Constants.Paging.DefaultPageSize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(_pagingstrategy, routeValueDictionary[QueryParam.PagingDashStrategy]);
        }

        [Fact]
        public void GenerateRouteValues_Without_PagingStrategy_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator(usePagingStrategy: false);

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(PagingStrategy.WithCount, routeValueDictionary[QueryParam.PagingDashStrategy]);
        }

        [Fact]
        public void GenerateRouteValues_With_Null_PagingStrategy_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator(usePagingStrategy: false);

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new Models.PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = null
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Null(routeValueDictionary[QueryParam.PagingDashStrategy]);
        }

        [Fact]
        public void GenerateRouteValues_With_Incorrect_PagingStrategy_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator(useIncorrectSyntaxPagingStrategy: true);

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy,
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(_pagingstrategy, routeValueDictionary[QueryParam.PagingDashStrategy]);
            Assert.Null(routeValueDictionary[QueryParam.PagingStrategy]);
        }

        [Fact]
        public void GenerateRouteValues_With_Sort_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator();

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy,
                Sort = _sort
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(_pagingstrategy, routeValueDictionary[QueryParam.PagingDashStrategy]);
            Assert.Equal(_sort, routeValueDictionary[QueryParam.Sort]);
        }

        [Fact]
        public void GenerateRouteValues_Without_Sort_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator();

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(_pagingstrategy, routeValueDictionary[QueryParam.PagingDashStrategy]);
            Assert.Equal(Constants.Sorting.Default, routeValueDictionary[QueryParam.Sort]);
        }

        [Fact]
        public void GenerateRouteValues_With_Null_Sort_Returns_Route_Values()
        {
            var linkGenerator = SetupLinkGenerator();

            var pageNumberToGenerate = _page + 3;
            var routeValueDictionary = linkGenerator.GenerateRouteValues(new PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy,
                Sort = null
            },
            pageNumberToGenerate: pageNumberToGenerate,
            null);

            Assert.NotNull(routeValueDictionary);
            Assert.NotEmpty(routeValueDictionary);
            Assert.Equal(pageNumberToGenerate, routeValueDictionary[QueryParam.Page]);
            Assert.Equal(_pagesize, routeValueDictionary[QueryParam.PageSize]);
            Assert.Equal(_pagingstrategy, routeValueDictionary[QueryParam.PagingDashStrategy]);
            Assert.Null(routeValueDictionary[QueryParam.Sort]);
        }


        [Fact]
        public void GenerateLink_With_Route_For_PageOptions_Returns_Link_With_Href()
        {
            var linkGenerator = SetupLinkGenerator(useSort: false);

            var link = linkGenerator.GenerateLink(new PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy
            },
                page: _page,
                routeName: "a",
                routeValues: null
                );


            Assert.NotNull(link);
            Assert.NotNull(link.Href);
            Assert.Equal(GetFullUri(useSort: false), link.Href);
        }

        [Fact]
        public void GenerateLink_With_Route_For_PageSortOptions_Returns_Link_With_Href()
        {
            var linkGenerator = SetupLinkGenerator();

            var link = linkGenerator.GenerateLink(new PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy,
                Sort = _sort
            },
                page: _page,
                routeName: "a",
                routeValues: null
                );


            Assert.NotNull(link);
            Assert.NotNull(link.Href);
            Assert.Equal(GetFullUri(), link.Href);
        }

        [Fact]
        public void GenerateLink_With_Action_For_PageOptions_Returns_Link_With_Href()
        {
            var linkGenerator = SetupLinkGenerator(useSort: false);

            var link = linkGenerator.GenerateLink(new PageOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy
            },
                page: _page,
                actionName: "a",
                controllerName: "b",
                routeValues: null
                );


            Assert.NotNull(link);
            Assert.NotNull(link.Href);
            Assert.Equal(GetFullUri(useSort: false), link.Href);
        }

        [Fact]
        public void GenerateLink_With_Action_For_PageSortOptions_Returns_Link_With_Href()
        {
            var linkGenerator = SetupLinkGenerator();

            var link = linkGenerator.GenerateLink(new PageSortOptions
            {
                Page = _page,
                PageSize = _pagesize,
                PagingStrategy = _pagingstrategy,
                Sort = _sort
            },
                page: _page,
                actionName: "a",
                controllerName: "b",
                routeValues: null
                );


            Assert.NotNull(link);
            Assert.NotNull(link.Href);
            Assert.Equal(GetFullUri(), link.Href);
        }
    }
}
