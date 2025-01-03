using Domain.Services;

namespace MyAppAPI.HostedServices
{
    public class OrderCheckingService(
        IServiceProvider _services, 
        ILogger<OrderCheckingService> _logger) : IHostedService, IDisposable
    {
        private Timer _timer = null;
        private bool _disposed;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Order Checking Service is running.");

            _timer = new Timer(TryInitialiseOrderChecking, null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private async void TryInitialiseOrderChecking(object state)
        {
            _logger.LogInformation("TryInitialiseOrderChecking is running.");

            using var scope = _services.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IOrderEventsService>();

            var success = await service.TryInitialiseSupervisor();

            if (success)
            {
                _logger.LogInformation("Order Supervisor is running.");
                _timer?.Change(Timeout.Infinite, 0);

                return;
            }

            _logger.LogWarning("Order Supervisor is not running, will try again...");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Order Checking Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
