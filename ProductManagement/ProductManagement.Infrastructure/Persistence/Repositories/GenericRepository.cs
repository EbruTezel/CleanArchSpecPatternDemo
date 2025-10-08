using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductManagement.Application.Persistence.Repositories;
using ProductManagement.Application.Persistence.Specifications;
using ProductManagement.Domain.Abstractions;
using ProductManagement.Infrastructure.Persistence.Specifications;

namespace ProductManagement.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity, TDbContext> : IGenericRepository<TEntity>
    where TEntity : BaseEntity
    where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;
    private IDbContextTransaction? _transaction;

    protected GenericRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<TEntity>() ?? throw new ArgumentNullException(nameof(dbContext), "DbSet cannot be null");
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public TEntity Update(TEntity entity)
    {
        DbContext.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public TEntity Delete(TEntity entity)
    {
        DbSet.Remove(entity);
        return entity;
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }

    // Specification-based methods
    public async Task<IList<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var queryable = ApplySpecification(spec);
        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<IList<TResult>> ListAsync<TResult>(ISpecification<TEntity> spec, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        var queryable = ApplySpecification(spec);
        return await queryable.Select(selector).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var queryable = ApplySpecification(spec);
        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var queryable = ApplySpecification(spec, ignorePaging: true);
        return await queryable.CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var queryable = ApplySpecification(spec, ignorePaging: true);
        return await queryable.AnyAsync(cancellationToken);
    }

    public async Task<(IList<TEntity> Items, int TotalCount)> ListWithTotalCountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        var totalQuery = ApplySpecification(spec, ignorePaging: true);
        var totalCount = await totalQuery.CountAsync(cancellationToken);
        var items = await query.ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<(IList<TResult> Items, int TotalCount)> ListWithTotalCountAsync<TResult>(ISpecification<TEntity> spec, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec).Select(selector);
        var totalQuery = ApplySpecification(spec, ignorePaging: true);
        var totalCount = await totalQuery.CountAsync(cancellationToken);
        var items = await query.ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec, bool ignorePaging = false)
    {
        return SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), spec, ignorePaging);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DbContext.SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task<TResult> ExecuteTransactionAsync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await func();
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task ExecuteTransactionAsync(Func<Task> func, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await func();
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TEntity> BuildQuery(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool asNoTracking = false)
    {
        IQueryable<TEntity> query = DbSet;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (includes != null && includes.Any())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }
}