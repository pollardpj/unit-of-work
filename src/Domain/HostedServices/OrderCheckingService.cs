using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Domain.HostedServices
{
    public class OrderCheckingService(
        IServiceProvider _services, 
        ILogger<OrderCheckingService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(token))
            {
                if (await TryInitialiseOrderChecking())
                {
                    return;
                }
            }
        }

        private async Task<bool> TryInitialiseOrderChecking()
        {
            using var scope = _services.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IOrderEventsService>();

            var success = await service.TryInitialiseSupervisor();

            if (success)
            {
                _logger.LogInformation("Order Supervisor is running.");
                return true;
            }

            _logger.LogWarning("Order Supervisor is not running, will try again...");
            return false;
        }
    }
}
