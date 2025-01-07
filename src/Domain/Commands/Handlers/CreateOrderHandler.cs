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
            Reference = Guid.CreateVersion7(),
            ProductName = command.ProductName,
            Price = 2.99M
        };

        order.Events.Add(new OrderEvent
        {
            Reference = order.Reference,
            CreatedTimestampUtc = DateTime.UtcNow,
            Status = EventStatus.Pending,
            Type = OrderEventType.Created,
            Payload = JsonSerializer.Serialize(new OrderEventPayload
            {
                Reference = Guid.CreateVersion7(),
                Type = OrderEventType.Created,
                ProductName = order.ProductName,
                Price = order.Price
            }, JsonHelpers.DefaultOptions)
        });

        using (var unitOfWork = _unitOfWorkFactory.Create())
        {
            unitOfWork.OrderRepository.Add(order);
            await unitOfWork.FlushAsync(token);
        }

        await _eventsService.TryPublishEvents(order.Reference, token);

        return new CreateOrderResult
        {
            Reference = order.Reference,
            Price = order.Price
        };
    }
}
