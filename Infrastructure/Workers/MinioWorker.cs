using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Realchat.Application.Workers;

namespace Realchat.Infrastructure.Workers;

public class MinioWorker : BackgroundService
{
    private readonly ILogger<MinioWorker> _logger;
    public IBackgroundTaskQueue BackgroundTaskQueue { get; set; }

    public MinioWorker(ILogger<MinioWorker> logger, IBackgroundTaskQueue backgroundTaskQueue)
    {
        _logger = logger;
        BackgroundTaskQueue = backgroundTaskQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Minio Worker is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await BackgroundTaskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
            }
        }
        _logger.LogInformation("Minio Worker is stopping.");
    }
}