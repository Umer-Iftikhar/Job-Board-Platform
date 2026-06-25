using JobBoard.Api.Application.Interfaces;

namespace JobBoard.Api.Infrastructure.Services
{
    public class EmailProcessingService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<EmailProcessingService> _logger;

        public EmailProcessingService(IBackgroundTaskQueue taskQueue, ILogger<EmailProcessingService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing email task.");
                }
            }
        }
    }
}
