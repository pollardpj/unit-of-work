namespace Domain.Services
{
    public interface IOrderEventsService
    {
        ValueTask EnsurePublishEvents(Guid orderReference);
    }
}
