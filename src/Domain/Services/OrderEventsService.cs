﻿using Dapr.Actors;
using Dapr.Actors.Client;
using Domain.Actors;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public class OrderEventsService(ILogger<OrderEventsService> _logger) : IOrderEventsService
{
    public async ValueTask<bool> TryPublishEvents(Guid orderId, CancellationToken token = default)
    {
        try
        {
            var actorId = new ActorId($"order-{orderId}");
            var proxy = ActorProxy.Create<IOrderActor>(actorId, nameof(OrderActor));
            await proxy.PublishEvents(orderId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public async ValueTask<bool> TryInitialiseSupervisor(CancellationToken token = default)
    {
        try
        {
            var actorId = new ActorId("order-supervisor");
            var proxy = ActorProxy.Create<IOrderSupervisorActor>(actorId, nameof(OrderSupervisorActor));
            await proxy.StartCheckingOrders();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}
