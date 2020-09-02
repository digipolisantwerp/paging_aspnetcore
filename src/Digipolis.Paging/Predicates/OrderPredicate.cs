using Digipolis.Paging.Constants;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Digipolis.Paging.Predicates
{
    /// <summary>
    /// IQueryable extension methods for sorting
    /// </summary>
    public static class OrderPredicate
    {
        /// <summary>
        /// Order an IQueryable
        /// </summary>
        /// <param name="query">The given IQueryable</param>
        /// <param name="sortingString">
        /// Comma seperated list containing the names of the properties to sort by. 
        /// In order to sort descending, set '-' in front of the property name (e.g. "-id"). 
        /// Default value is "id" if class contains a property named 'Id'. If there is no such property, OrderBy will sort on the first property found by default</param>
        /// <returns>An IOrderedQueryable</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortingString = null) where T : class
        {
            var entityType = typeof(T);
            var sortingFields = (sortingString ?? string.Empty).Split(',');

            if (sortingFields == null || sortingFields.All(x => string.IsNullOrWhiteSpace(x)))
            {
                if (entityType.GetProperty("id") != null || entityType.GetProperty("Id") != null || entityType.GetProperty("ID") != null)
                {
                    sortingFields = new string[] { Sorting.Default };
                }
                else
                {
                    var firstProperty = entityType.GetProperties().FirstOrDefault()?.Name;
                    if (string.IsNullOrWhiteSpace(firstProperty))
                    {
                        return (IOrderedQueryable<T>)query;
                    }
                    sortingFields = new string[] { firstProperty };
                }
            }

            for (int i = 0; i < sortingFields.Length; i++)
            {
                bool orderByDescending = sortingFields[i].StartsWith("-");
                var sortingField = orderByDescending ? sortingFields[i].Split('-').Last() : sortingFields[i];

                //Create x=>x.PropName
                var propertyInfo = entityType.GetProperty(sortingField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    throw new ArgumentException($"No property '{sortingField}' in + {typeof(T).Name}'");

                ParameterExpression arg = Expression.Parameter(entityType, "x");
                MemberExpression property = Expression.Property(arg, sortingField);
                var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

                //Check if an order method has already been added. If so => change method to ThenBy/ThenByDescending
                string orderByMethodName;
                if (!orderByDescending)
                    orderByMethodName = i == 0 ? "OrderBy" : "ThenBy";
                else
                    orderByMethodName = i == 0 ? "OrderByDescending" : "ThenByDescending";

                //Get System.Linq.Queryable.OrderBy() method.
                var enumerableType = typeof(Queryable);
                var method = enumerableType.GetMethods()
                     .Where(m => m.Name == orderByMethodName && m.IsGenericMethodDefinition)
                     .Single(m =>
                     {
                         var parameters = m.GetParameters().ToList();
                         //Put more restriction here to ensure selecting the right overload                
                         return parameters.Count == 2;//overload that has 2 parameters
                     });

                //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
                MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

                /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
                  Note that we pass the selector as Expression to the method and we don't compile it.
                  By doing so EF can extract "order by" columns and generate SQL for it.*/
                query = (IOrderedQueryable<T>)genericMethod
                    .Invoke(genericMethod, new object[] { query, selector });
            }

            return (IOrderedQueryable<T>)query;
        }
    }
}
