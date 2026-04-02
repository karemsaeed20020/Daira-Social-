using Daira.Application.Interfaces;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Daira.Infrastructure.Specefication
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecefication<T> spec)
        {
            var query = inputQuery;
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // To Sort Data
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDesc != null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }

            // Pagination
            if (spec.HasPagination)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            // Now Add Includes To Current Query
            query = spec.Includes
                .Aggregate(query, (currentQuery, IncludeExpression)
                => currentQuery.Include(IncludeExpression));

            // Add string-based includes (for nested)
            query = spec.IncludeStrings
                .Aggregate(query, (currentQuery, includeString)
                => currentQuery.Include(includeString));


            return query;
        }
    }
}
