using Digipolis.Paging.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Digipolis.Paging.Mapping
{
    /// <summary>
    /// Convert data to a paged result 
    /// </summary>
    public class PageConverter : IPageConverter
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILinkGenerator _linkGenerator;

        public PageConverter(IActionContextAccessor actionContextAccessor, ILinkGenerator linkGenerator)
        {
            _actionContextAccessor = actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        /// <summary>
        /// Convert a DataPage to a paged result 
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="dataPage">List of data objects of type T with paging information</param>
        /// <returns>A paged result based on the HAL specification</returns>
        public PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageSortOptions pageSortOptions, DataPage<T> dataPage)
            where TEmbedded : IEmbedded<T>, new()
            where T : class
        {
            return ToPagedResult<T, TEmbedded>(pageSortOptions, dataPage?.Data ?? Enumerable.Empty<T>(), dataPage.TotalEntityCount);
        }

        /// <summary>
        /// Convert a DataPage to a paged result 
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="dataPage">List of data objects of type T with paging information</param>
        /// <returns>A paged result based on the HAL specification</returns>
        public PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageOptions pageOptions, DataPage<T> dataPage)
            where TEmbedded : IEmbedded<T>, new()
            where T : class
        {
            return ToPagedResult<T, TEmbedded>(pageOptions, dataPage?.Data ?? Enumerable.Empty<T>(), dataPage.TotalEntityCount);
        }

        /// <summary>
        /// Convert data to a paged result 
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="data">IEnumerable of data objects of type T</param>
        /// <param name="total">Total result count</param>
        /// <returns>A paged result based on the HAL specification</returns>
        public PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageSortOptions pageSortOptions, IEnumerable<T> data, long? total)
            where TEmbedded : IEmbedded<T>, new()
            where T : class
        {
            var descriptor = (ControllerActionDescriptor)_actionContextAccessor.ActionContext.ActionDescriptor;
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerName;
            var routeValues = _actionContextAccessor.ActionContext.RouteData.Values;
            var page = pageSortOptions?.Page ?? Constants.Paging.DefaultPageNumber;

            var result = new PagedResult<T, TEmbedded>(page, pageSortOptions?.PageSize ?? Constants.Paging.DefaultPageSize, total, data);

            result.Links.Self = _linkGenerator.GenerateLink(pageSortOptions, page, actionName, controllerName, routeValues);
            result.Links.First = _linkGenerator.GenerateLink(pageSortOptions, 1, actionName, controllerName, routeValues);
            // last page
            result.Links.Last = _linkGenerator.GenerateLink(pageSortOptions, result.Page.TotalPages, actionName, controllerName, routeValues);
            // previous page: if current page = 1, there is no previous page
            result.Links.Previous = (page - 1 >= 1) ? _linkGenerator.GenerateLink(pageSortOptions, page - 1, actionName, controllerName, routeValues) : null;
            // next page: if current page is last page, there is no next page
            result.Links.Next = (page + 1 <= result.Page.TotalPages) ? _linkGenerator.GenerateLink(pageSortOptions, page + 1, actionName, controllerName, routeValues) : null;

            return result;
        }

        /// <summary>
        /// Convert data to a paged result 
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="data">IEnumerable of data objects of type T</param>
        /// <param name="total">Total result count</param>
        /// <returns>A paged result based on the HAL specification</returns>
        public PagedResult<T, TEmbedded> ToPagedResult<T, TEmbedded>(PageOptions pageOptions, IEnumerable<T> data, long? total)
            where TEmbedded : IEmbedded<T>, new()
            where T : class
        {
            var descriptor = (ControllerActionDescriptor)_actionContextAccessor.ActionContext.ActionDescriptor;
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerName;
            var routeValues = _actionContextAccessor.ActionContext.RouteData.Values;
            var page = pageOptions?.Page ?? Constants.Paging.DefaultPageNumber;

            var result = new PagedResult<T, TEmbedded>(page, pageOptions?.PageSize ?? Constants.Paging.DefaultPageSize, total, data)
            {
                Links =
                {
                    First = _linkGenerator.GenerateLink(pageOptions, 1, actionName, controllerName, routeValues),
                    Self = _linkGenerator.GenerateLink(pageOptions, page, actionName, controllerName, routeValues),
                    Last = null,
                    // previous page: if current page = 1, there is no previous page
                    Previous = (page - 1 >= 1) ? _linkGenerator.GenerateLink(pageOptions, page, actionName, controllerName, routeValues) : null,
                    Next = null
                }
            };
            // last page
            result.Links.Last = _linkGenerator.GenerateLink(pageOptions, result.Page.TotalPages, actionName, controllerName, routeValues);

            // next page: if current page is last page, there is no next page
            if (page + 1 <= result.Page.TotalPages)
                result.Links.Next = _linkGenerator.GenerateLink(pageOptions, page + 1, actionName, controllerName, routeValues);

            return result;
        }
    }
}
