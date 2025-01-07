namespace Domain.Services;

public interface IOrderEventsService
{
    ValueTask<bool> TryInitialiseSupervisor(CancellationToken token = default);
    ValueTask<bool> TryPublishEvents(Guid orderId, CancellationToken token = default);
}
