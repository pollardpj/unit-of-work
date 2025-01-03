using Dapr.Actors.Runtime;
using Domain.Services;
using Domain.UnitOfWork;

namespace Domain.Actors;

public class OrderSupervisorActor(
    ActorHost _host,
    IMyAppUnitOfWorkFactory _unitOfWorkFactory,
    IOrderEventsService _eventsService) : Actor(_host), IOrderSupervisorActor, IRemindable
{
    public async Task StartCheckingOrders()
    {
        await RegisterReminderAsync("CheckOrders", null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
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