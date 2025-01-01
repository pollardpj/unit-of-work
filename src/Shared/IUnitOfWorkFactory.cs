namespace Shared;

public interface IUnitOfWorkFactory<TUnitOfWork> where TUnitOfWork : IUnitOfWork
{
    TUnitOfWork Create();
}
