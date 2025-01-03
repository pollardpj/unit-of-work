using Dapr.Actors.Runtime;
using Domain.Services;
using Domain.UnitOfWork;

namespace Domain.Actors;

public class OrderSupervisorActor(
    ActorHost _host,
    IMyAppUnitOfWorkFactory _unitOfWorkFactory,
    IOrderEventsService _eventsService) : Actor(_host), IOrderSupervisorActor
{
    public async Task StartCheckingOrders()
    {
        await RegisterTimerAsync("CheckOrders", nameof(ReceiveTimerAsync), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public async Task ReceiveTimerAsync(byte[] state)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var orderReferences = unitOfWork.OrderRepository
            .GetOrderReferencesWithPendingEvents(DateTime.UtcNow.AddSeconds(-10));

        await foreach (var orderReference in orderReferences)
        {
            await _eventsService.TryPublishEvents(orderReference);
        }
    }
}