namespace Domain.Services
{
    public interface IOrderEventsService
    {
        ValueTask EnsurePublishEvents(Guid orderReference, CancellationToken token = default);
    }
}
