using Dapr.Actors;

namespace Domain.Actors;

public interface IOrderSupervisorActor : IActor
{
    Task StartCheckingOrders();
}