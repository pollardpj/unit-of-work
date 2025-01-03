using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Services;
using Domain.UnitOfWork;
using Shared;
using Shared.CQRS;
using System.Text.Json;

namespace Domain.Commands.Handlers
{
    public class CreateOrderHandler(
        IMyAppUnitOfWorkFactory _unitOfWorkFactory,
        IOrderEventsService _eventsService) : ICommandHandler<CreateOrder>
    {
        public async ValueTask ExecuteAsync(CreateOrder command, CancellationToken token = default)
        {
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

            command.Price = order.Price;

            await _eventsService.EnsurePublishEvents(command.Reference, token);
        }
    }
}
