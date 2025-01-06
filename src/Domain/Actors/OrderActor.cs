using Dapr.Actors.Runtime;
using Dapr.Client;
using Domain.Enums;
using Domain.Events;
using Domain.UnitOfWork;
using Microsoft.Extensions.Logging;
using Shared.Json;
using System.Text;
using System.Text.Json;

namespace Domain.Actors;

public class OrderActor(
    ActorHost _host,
    IMyAppUnitOfWorkFactory _unitOfWorkFactory,
    ILogger<OrderActor> _logger) : Actor(_host), IOrderActor
{
    public async Task PublishEvents(Guid orderReference)
    {
        _logger.LogInformation("Registering Timer for {OrderReference}", orderReference);

        await RegisterTimerAsync("PublishEvents", 
            nameof(ReceiveTimerAsync),
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new PublishEventsTimerData
            {
                OrderReference = orderReference
            }, JsonHelpers.DefaultOptions)), 
            TimeSpan.FromMilliseconds(1), 
            TimeSpan.FromMilliseconds(-1));
    }

    public async Task ReceiveTimerAsync(byte[] state)
    {
        var data = JsonSerializer.Deserialize<PublishEventsTimerData>(state, JsonHelpers.DefaultOptions);

        using var client = new DaprClientBuilder()
            .UseJsonSerializationOptions(JsonHelpers.DefaultOptions)
            .Build();

        using var unitOfWork = _unitOfWorkFactory.Create();

        _logger.LogInformation("Processing Events for {OrderReference}", data.OrderReference);

        var orderEvents = await unitOfWork.OrderEventRepository.GetPendingEvents(data.OrderReference);

        foreach (var orderEvent in orderEvents)
        {
            var payload = JsonSerializer.Deserialize<OrderEventPayload>(
                orderEvent.Payload, JsonHelpers.DefaultOptions);

            try
            {
                await client.PublishEventAsync("order-pubsub", "order-event", payload);

                orderEvent.Status = EventStatus.Published;
                unitOfWork.OrderEventRepository.Update(orderEvent);
                await unitOfWork.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error on publishing event for order with reference {OrderReference}", data.OrderReference);
            }
        }
    }

    private class PublishEventsTimerData
    {
        public Guid OrderReference { get; set; }
    }
}