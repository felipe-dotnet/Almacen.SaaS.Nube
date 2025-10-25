using Almacen.Saas.Domain.Interfaces;

namespace Almacen.Saas.Infraestructure.Repositories;
public interface IUnitOfWork:IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

