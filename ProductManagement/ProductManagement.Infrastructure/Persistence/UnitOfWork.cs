using ProductManagement.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;
using ProductManagement.Application.Persistence;
using IProductRepository = ProductManagement.Application.Persistence.Repositories.IProductRepository;

namespace ProductManagement.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public IProductRepository ProductRepository { get; }

    public UnitOfWork(ApplicationDbContext dbContext, IProductRepository productRepository)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        ProductRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
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
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<TResult> ExecuteTransactionAsync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
    {
        try
        {
            await BeginTransactionAsync(cancellationToken);
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
        try
        {
            await BeginTransactionAsync(cancellationToken);
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
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _dbContext.Dispose();
            }
        }

        _disposed = true;
    }
}