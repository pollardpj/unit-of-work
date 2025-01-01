using Dapr.Actors;
using Dapr.Actors.Client;
using Domain.Actors;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public class OrderEventsService(ILogger<OrderEventsService> logger) : IOrderEventsService
{
    public async ValueTask EnsurePublishEvents(Guid orderReference)
    {
        try
        {
            var actorId = new ActorId($"order-{orderReference}");
            var proxy = ActorProxy.Create<IOrderActor>(actorId, nameof(OrderActor));
            await proxy.PublishEvents(orderReference);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }
}
