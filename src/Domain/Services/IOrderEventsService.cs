namespace Domain.Services;

public interface IOrderEventsService
{
    ValueTask<bool> TryInitialiseSupervisor(CancellationToken token = default);
    ValueTask<bool> TryPublishEvents(Guid orderReference, CancellationToken token = default);
}
