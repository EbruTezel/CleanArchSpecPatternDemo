using ProductManagement.Application.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.Infrastructure.Persistence.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec, bool ignorePaging = false) where TEntity : class
    {
        var query = inputQuery;

        if (spec.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (spec.AsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        if (spec.Criteria is not null)
        {
            query = query.Where(spec.Criteria);
        }

        if (spec.OrderBy is not null)
        {
            query = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending is not null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        foreach (var includeExpression in spec.Includes)
        {
            query = query.Include(includeExpression);
        }

        foreach (var includeString in spec.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        if (!ignorePaging && spec.IsPagingEnabled)
        {
            if (spec.Skip.HasValue)
            {
                query = query.Skip(spec.Skip.Value);
            }
            if (spec.Take.HasValue)
            {
                query = query.Take(spec.Take.Value);
            }
        }

        return query;
    }
}
