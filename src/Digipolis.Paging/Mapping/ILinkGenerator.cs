using Digipolis.Paging.Models;

namespace Digipolis.Paging.Mapping
{
    public interface ILinkGenerator
    {
        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the executed action
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="actionName">Name of the executed action</param>
        /// <param name="controllerName">Name of the controller containing the action method</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        Link GenerateLink(PageSortOptions pageSortOptions, int? page, string actionName, string controllerName, object routeValues = null);

        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the executed action
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="actionName">Name of the executed action</param>
        /// <param name="controllerName">Name of the controller containing the action method</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        Link GenerateLink(PageOptions pageOptions, int? page, string actionName, string controllerName, object routeValues = null);

        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the route of executed action
        /// </summary>
        /// <param name="pageSortOptions">Options for paging and sorting</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="routeName">Name of the executed route</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        Link GenerateLink(PageSortOptions pageSortOptions, int? page, string routeName, object routeValues = null);
        
        /// <summary>
        /// Generate a link to a specific page based on paging and sorting information, and the route of executed action
        /// </summary>
        /// <param name="pageOptions">Options for paging</param>
        /// <param name="page">Page number. Default = 1</param>
        /// <param name="routeName">Name of the executed route</param>
        /// <param name="routeValues">Route data</param>
        /// <returns>A link containing an url in the href property</returns>
        Link GenerateLink(PageOptions pageOptions, int? page, string routeName, object routeValues = null);
    }
}