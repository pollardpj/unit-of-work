using Dapr.Actors;

namespace Domain.Actors;

public interface IOrderActor : IActor
{
    Task PublishEvents(Guid orderId);
}