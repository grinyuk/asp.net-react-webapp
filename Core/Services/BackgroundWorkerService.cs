using Core.Entities;
using Core.Helpers;
using Core.Interfaces.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class BackgroundWorkerService : IHostedService, IDisposable
    {
        private Timer? _timer = null;

        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppConfig _appConfig;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger
            , IServiceProvider serviceProvider
            , AppConfig appConfig)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _appConfig = appConfig;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running." + DateTime.Now);

            _timer = new Timer(RefreshUserScore, null, TimeSpan.Zero,
            _appConfig.RecalculateUserScoreLifeSpan);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void RefreshUserScore(object? state)
        {
            _logger.LogInformation("RefreshUserScore" + DateTime.Now);
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var calculateService = scope.ServiceProvider.GetRequiredService<ICalculateService>();

                    calculateService.StartScoreCalculateAsync().Wait();
                    calculateService.StartArchiveScoreCalculateAsync().Wait();
                }
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(BackgroundWorkerService) + Constants.ExceptionPlusArrow + nameof(RefreshUserScore), ex);
            }

        }
    }
}
