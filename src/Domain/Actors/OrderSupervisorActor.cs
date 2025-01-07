using Dapr.Actors.Runtime;
using Domain.Services;
using Domain.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Domain.Actors;

public class OrderSupervisorActor(
    ActorHost _host,
    IMyAppUnitOfWorkFactory _unitOfWorkFactory,
    IOrderEventsService _eventsService,
    ILogger<OrderSupervisorActor> _logger) : Actor(_host), IOrderSupervisorActor
{
    public async Task StartCheckingOrders()
    {
        _logger.LogInformation("Registering Timer for Checking Orders");

        await RegisterTimerAsync("CheckOrders", nameof(ReceiveTimerAsync), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public async Task ReceiveTimerAsync(byte[] state)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        _logger.LogInformation("Checking Orders");

        var orderIds = unitOfWork.OrderRepository
            .GetOrderIdsWithPendingEvents(DateTime.UtcNow.AddSeconds(-10));

        await foreach (var orderId in orderIds)
        {
            await _eventsService.TryPublishEvents(orderId);
        }
    }
}