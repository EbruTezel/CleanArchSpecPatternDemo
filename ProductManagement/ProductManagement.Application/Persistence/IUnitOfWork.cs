
using ProductManagement.Application.Persistence.Repositories;

namespace ProductManagement.Application.Persistence;

public interface IUnitOfWork : IDisposable
{
    IProductRepository ProductRepository { get; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<TResult> ExecuteTransactionAsync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default);
    Task ExecuteTransactionAsync(Func<Task> func, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}