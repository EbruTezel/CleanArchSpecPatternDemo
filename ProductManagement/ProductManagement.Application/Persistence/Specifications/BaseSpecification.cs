using System.Linq.Expressions;

namespace ProductManagement.Application.Persistence.Specifications;

public abstract class BaseSpecification<TEntity> : ISpecification<TEntity>
{
    public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }

    protected BaseSpecification()
    {
    }

    protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
    {
        Criteria = criteria;
    }

    public List<Expression<Func<TEntity, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();

    public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; protected set; }

    public int? Take { get; protected set; }
    public int? Skip { get; protected set; }
    public bool IsPagingEnabled { get; protected set; }

    public bool AsNoTracking { get; protected set; }
    public bool AsSplitQuery { get; protected set; }

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression) => Includes.Add(includeExpression);
    protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);

    protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression) => OrderBy = orderByExpression;
    protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression) => OrderByDescending = orderByDescExpression;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected void ApplyAsNoTracking() => AsNoTracking = true;
    protected void ApplyAsSplitQuery() => AsSplitQuery = true;

    public BaseSpecification<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        Criteria = Criteria is null ? predicate : Criteria.And(predicate);
        return this;
    }

    public BaseSpecification<TEntity> Or(Expression<Func<TEntity, bool>> predicate)
    {
        Criteria = Criteria is null ? predicate : Criteria.Or(predicate);
        return this;
    }

    public BaseSpecification<TEntity> WhereIf(bool condition, Expression<Func<TEntity, bool>> predicate)
        => condition ? Where(predicate) : this;

    public BaseSpecification<TEntity> OrIf(bool condition, Expression<Func<TEntity, bool>> predicate)
        => condition ? Or(predicate) : this;

    protected BaseSpecification<TEntity> ResetCriteria()
    {
        Criteria = null;
        return this;
    }
}

internal static class SpecificationExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        => Combine(left, right, Expression.AndAlso);

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        => Combine(left, right, Expression.OrElse);

    private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, Func<Expression, Expression, BinaryExpression> merge)
    {
        var parameter = left.Parameters[0];
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body);
        var body = merge(left.Body, rightBody!);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == source ? target : base.VisitParameter(node);
    }
}
