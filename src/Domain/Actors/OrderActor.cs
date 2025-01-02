using Dapr.Actors.Runtime;
using Dapr.Client;
using Domain.Enums;
using Domain.Events;
using Domain.UnitOfWork;
using Microsoft.Extensions.Logging;
using Shared;
using System.Text;
using System.Text.Json;

namespace Domain.Actors;

public class OrderActor(
    ActorHost host,
    IMyAppUnitOfWorkFactory unitOfWorkFactory,
    ILogger<OrderActor> logger) : Actor(host), IOrderActor, IRemindable
{
    public async Task PublishEvents(Guid orderReference)
    {
        await RegisterReminderAsync("PublishEvents", 
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new PublishEventsReminderData
            {
                OrderReference = orderReference
            }, JsonHelpers.DefaultOptions)), 
            TimeSpan.FromMilliseconds(1), 
            TimeSpan.FromMilliseconds(-1));
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        var data = JsonSerializer.Deserialize<PublishEventsReminderData>(state, JsonHelpers.DefaultOptions);

        using var client = new DaprClientBuilder()
            .UseJsonSerializationOptions(JsonHelpers.DefaultOptions)
            .Build();

        using var unitOfWork = unitOfWorkFactory.Create();

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
                logger.LogWarning(ex, "Error on publishing event for order with reference {OrderReference}", data.OrderReference);
            }
        }
    }

    private class PublishEventsReminderData
    {
        public Guid OrderReference { get; set; }
    }
}