using System.Linq.Expressions;
using ProductManagement.Application.Persistence.Specifications;
using ProductManagement.Domain.Abstractions;

namespace ProductManagement.Application.Persistence.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    TEntity Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    TEntity Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    // Specification-based API
    Task<IList<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<IList<TResult>> ListAsync<TResult>(ISpecification<TEntity> spec, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<(IList<TEntity> Items, int TotalCount)> ListWithTotalCountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<(IList<TResult> Items, int TotalCount)> ListWithTotalCountAsync<TResult>(ISpecification<TEntity> spec, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);
}