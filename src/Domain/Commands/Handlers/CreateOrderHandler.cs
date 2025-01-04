using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Services;
using Domain.UnitOfWork;
using Shared;
using Shared.CQRS;
using Shared.Observability;
using System.Text.Json;

namespace Domain.Commands.Handlers
{
    public class CreateOrderHandler(
        IMyAppUnitOfWorkFactory _unitOfWorkFactory,
        IOrderEventsService _eventsService) : ICommandHandler<CreateOrder, CreateOrderResult>
    {
        public async ValueTask<CreateOrderResult> ExecuteAsync(CreateOrder command, CancellationToken token = default)
        {
            using var _ = TracingHelpers.StartActivity(nameof(CreateOrderHandler));

            var order = new Order
            {
                Reference = command.Reference,
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
                    Reference = order.Reference,
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

            await _eventsService.TryPublishEvents(command.Reference, token);

            return new CreateOrderResult
            {
                Price = order.Price
            };
        }
    }
}
