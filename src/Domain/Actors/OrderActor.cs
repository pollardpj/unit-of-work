using System.Text;
using System.Text.Json;
using Dapr.Actors.Runtime;
using Dapr.Client;
using Domain.Enums;
using Domain.Events;
using Domain.UnitOfWork;
using Microsoft.Extensions.Logging;
using Shared.Json;

namespace Domain.Actors;

public class OrderActor(
    ActorHost _host,
    IMyAppUnitOfWorkFactory _unitOfWorkFactory,
    ILogger<OrderActor> _logger) : Actor(_host), IOrderActor
{
    public async Task PublishEvents(Guid orderId)
    {
        _logger.LogInformation("Registering Timer for {OrderId}", orderId);

        await RegisterTimerAsync("PublishEvents", 
            nameof(ReceiveTimerAsync),
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new PublishEventsTimerData
            {
                OrderId = orderId
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

        _logger.LogInformation("Processing Events for {OrderId}", data.OrderId);

        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync();

        var orderEvents = await unitOfWork.OrderEventRepository.GetPendingEvents(data.OrderId);

        foreach (var orderEvent in orderEvents)
        {
            var payload = JsonSerializer.Deserialize<OrderEventPayload>(
                orderEvent.Payload, JsonHelpers.DefaultOptions);

            try
            {
                await client.PublishEventAsync("order-pubsub", "order-event", payload, new Dictionary<string, string>
                {
                    ["cloudevent.id"] = orderEvent.Id.ToString(),
                    ["cloudevent.type"] = "uk.co.mpgen.myapp.orderevent.v1"
                });

                orderEvent.Status = EventStatus.Published;
                unitOfWork.OrderEventRepository.Update(orderEvent);
                await unitOfWork.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error on publishing event for order with Id {OrderId}", data.OrderId);
            }
        }
    }

    private class PublishEventsTimerData
    {
        public Guid OrderId { get; set; }
    }
}