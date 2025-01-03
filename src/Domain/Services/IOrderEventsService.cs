namespace Domain.Services
{
    public interface IOrderEventsService
    {
        ValueTask<bool> TryInitialiseSupervisor(CancellationToken token = default);
        ValueTask EnsurePublishEvents(Guid orderReference, CancellationToken token = default);
    }
}
