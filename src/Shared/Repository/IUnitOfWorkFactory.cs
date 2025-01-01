namespace Shared.Repository;

public interface IUnitOfWorkFactory<TUnitOfWork> where TUnitOfWork : IUnitOfWork
{
    TUnitOfWork Create();
}
