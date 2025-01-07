using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Services;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.CQRS;
using Shared.Exceptions;
using Shared.Json;

namespace Domain.Commands.Handlers;

public class UpdateOrderHandler(
    IMyAppUnitOfWorkFactory _unitOfWorkFactory,
    IOrderEventsService _eventsService) : ICommandHandler<UpdateOrder, UpdateOrderResult>
{
    public async ValueTask<UpdateOrderResult> ExecuteAsync(UpdateOrder command, CancellationToken token = default)
    {
        Order order = null;

        using (var unitOfWork = _unitOfWorkFactory.Create())
        {
            order = await unitOfWork.OrderRepository.GetByIdAsync(command.Id);

            order.ProductName = command.ProductName;
            order.Price+= 1M;

            unitOfWork.OrderRepository.SetOriginalRowVersion(order, command.RowVersion);

            var eventId = Guid.CreateVersion7();

            order.Events.Add(new OrderEvent
            {
                Id = eventId,
                CreatedTimestampUtc = DateTime.UtcNow,
                Status = EventStatus.Pending,
                Type = OrderEventType.Updated,
                Payload = JsonSerializer.Serialize(new OrderEventPayload
                {
                    Id = eventId,
                    Type = OrderEventType.Updated,
                    ProductName = order.ProductName,
                    Price = order.Price
                }, JsonHelpers.DefaultOptions)
            });

            try
            {
                await unitOfWork.FlushAsync(token);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConflictException("Error on updating Order", ex);
            }
        }

        await _eventsService.TryPublishEvents(order.Id, token);

        return new UpdateOrderResult
        {
            Id = order.Id,
            Price = order.Price,
            RowVersion = order.RowVersion
        };
    }
}
