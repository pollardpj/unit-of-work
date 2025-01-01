using Domain.UnitOfWork;
using Shared.Repository;

namespace Domain.UnitOfWork;

public interface IMyAppUnitOfWorkFactory : IUnitOfWorkFactory<IMyAppUnitOfWork>
{
}
