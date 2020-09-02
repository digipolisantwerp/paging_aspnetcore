using Digipolis.Paging.Constants;
using Digipolis.Paging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;

namespace Digipolis.Paging.Mapping
{
    /// <summary>
    /// Generate links for paging
    /// </summary>
    public class LinkGenerator : ILinkGenerator
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LinkGenerator(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
            _urlHelperFactory = urlHelperFactory ?? throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the executed action
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="actionName">Name of the executed action</param>
        /// <param name="controllerName">Name of the controller containing the action method</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        public Link GenerateLink(PageSortOptions pageSortOptions, int? page, string actionName, string controllerName, object routeValues = null)
        {
            var values = GenerateRouteValues(pageSortOptions, page, routeValues);
            var url = AbsoluteAction(actionName, controllerName, values);
            return new Link(url.ToLowerInvariant());
        }

        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the executed action
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="actionName">Name of the executed action</param>
        /// <param name="controllerName">Name of the controller containing the action method</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        public Link GenerateLink(PageOptions pageOptions, int? page, string actionName, string controllerName, object routeValues = null)
        {
            var values = GenerateRouteValues(pageOptions, page, routeValues);
            var url = AbsoluteAction(actionName, controllerName, values);
            return new Link(url.ToLowerInvariant());
        }

        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the route of executed action
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="routeName">Name of the executed route</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        public Link GenerateLink(PageSortOptions pageSortOptions, int? page, string routeName, object routeValues = null)
        {
            var values = GenerateRouteValues(pageSortOptions, page, routeValues);
            var url = AbsoluteRoute(routeName, values);
            return new Link(url.ToLowerInvariant());
        }

        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the route of executed action
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="routeName">Name of the executed route</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        public Link GenerateLink(PageOptions pageOptions, int? page, string routeName, object routeValues = null)
        {
            var values = GenerateRouteValues(pageOptions, page, routeValues);
            var url = AbsoluteRoute(routeName, values);
            return new Link(url.ToLowerInvariant());
        }

        internal RouteValueDictionary GenerateRouteValues(PageSortOptions pageSortOptions, int? pageNumberToGenerate, object routeValues = null)
        {
            var values = GenerateRouteValues((PageOptions)pageSortOptions, pageNumberToGenerate, routeValues);
            if (!string.IsNullOrWhiteSpace(pageSortOptions?.Sort))
            {
                values[QueryParam.Sort] = pageSortOptions.Sort.ToLowerInvariant();
            }
            return values;
        }

        internal RouteValueDictionary GenerateRouteValues(PageOptions requestedPageOptions, int? pageNumberToGenerate, object routeValues = null)
        {
            var values = new RouteValueDictionary(routeValues);
            var query = _actionContextAccessor.ActionContext.HttpContext.Request.Query;
            foreach (var item in query)
            {
                var keyLowered = item.Key.ToLowerInvariant();
                //Exclude paging strategy from response => this will be transformed to paging-strategy for API requirement purposes
                if (!keyLowered.Equals(QueryParam.PagingStrategy, StringComparison.CurrentCultureIgnoreCase))
                    values[keyLowered] = item.Value;
            }

            values[QueryParam.Page] = pageNumberToGenerate ?? Constants.Paging.DefaultPageNumber;
            values[QueryParam.PageSize] = requestedPageOptions?.PageSize ?? Constants.Paging.DefaultPageSize;
            if (requestedPageOptions?.PagingStrategy.HasValue ?? false)
                values[QueryParam.PagingDashStrategy] = requestedPageOptions.PagingStrategy;
            return values;
        }

        internal string AbsoluteAction(string actionName, string controllerName, object routeValues = null)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var relativeUrl = urlHelper.Action(new UrlActionContext
            {
                Action = actionName,
                Controller = controllerName,
                Values = routeValues
            });
            return GetFullUrlBuilder(relativeUrl).ToString();
        }

        internal string AbsoluteRoute(string routeName, object routeValues = null)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var relativeUrl = urlHelper.RouteUrl(new UrlRouteContext
            {
                RouteName = routeName,
                Values = routeValues
            });
            return GetFullUrlBuilder(relativeUrl).ToString();
        }

        internal UriBuilder GetFullUrlBuilder(string relativeUrl)
        {
            var result = GetAbsoluteUrlBuilder();

            var indexQ = relativeUrl.IndexOf('?');

            if (indexQ > 0)
            {
                result.Path = relativeUrl.Substring(0, indexQ);
                result.Query = relativeUrl.Substring(indexQ, relativeUrl.Length - indexQ);
            }
            else
                result.Path = relativeUrl;

            return result;
        }

        internal UriBuilder GetAbsoluteUrlBuilder()
        {
            HttpRequest request = _actionContextAccessor.ActionContext.HttpContext.Request;
            RequestHeaders headers = new RequestHeaders(request.Headers);
            var host = headers.Host.HasValue ? headers.Host.Host : request.Host.Value;
            var port = headers.Host.Port;
            if (port != null)
                return new UriBuilder(request.Scheme, host, port.Value);
            else
                return new UriBuilder(request.Scheme, host);

        }
    }
}
