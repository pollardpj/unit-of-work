using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Services;
using Domain.UnitOfWork;
using Shared.CQRS;
using Shared.Json;

namespace Domain.Commands.Handlers;

public class CreateOrderHandler(
    IMyAppUnitOfWorkFactory _unitOfWorkFactory,
    IOrderEventsService _eventsService) : ICommandHandler<CreateOrder, CreateOrderResult>
{
    public async ValueTask<CreateOrderResult> ExecuteAsync(CreateOrder command, CancellationToken token = default)
    {
        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            ProductName = command.ProductName,
            Price = 2.99M,
            CreatedTimestampUtc = DateTime.UtcNow,
            Email = command.Email
        };

        var eventId = Guid.CreateVersion7();

        order.Events.Add(new OrderEvent
        {
            Id = eventId,
            CreatedTimestampUtc = DateTime.UtcNow,
            Status = EventStatus.Pending,
            Type = OrderEventType.Created,
            Payload = JsonSerializer.Serialize(new OrderEventPayload
            {
                EventId = eventId,
                OrderId = order.Id,
                Type = OrderEventType.Created,
                ProductName = order.ProductName,
                Price = order.Price
            }, JsonHelpers.DefaultOptions)
        });

        await using (var unitOfWork = await _unitOfWorkFactory.CreateAsync())
        {
            unitOfWork.OrderRepository.Add(order);
            await unitOfWork.FlushAsync(token);
        }

        await _eventsService.TryPublishEvents(order.Id, token);

        return new CreateOrderResult
        {
            Id = order.Id,
            Price = order.Price
        };
    }
}
