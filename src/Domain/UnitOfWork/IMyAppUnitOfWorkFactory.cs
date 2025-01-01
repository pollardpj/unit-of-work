using Domain.UnitOfWork;
using Shared;

namespace Domain.UnitOfWork;

public interface IMyAppUnitOfWorkFactory : IUnitOfWorkFactory<IMyAppUnitOfWork>
{
}
