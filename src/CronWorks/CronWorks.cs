using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CronWorks
{
    public class CronWorks : BackgroundService
    {
        private static readonly TimeSpan PoolingTime = TimeSpan.FromSeconds(1);

        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<string, BackgroundTaskScheduler> _schedulers =
            new ConcurrentDictionary<string, BackgroundTaskScheduler>();

        public CronWorks(IServiceProvider serviceProvider, ILogger<CronWorks> logger)
        {
            _serviceProvider = serviceProvider;
            Logger = logger;
        }

        public ILogger Logger { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                Logger.LogInformation("'{ServiceName}' is stopping.", nameof(CronWorks));
            });

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await UpdateAsync(stoppingToken);


                    await RunAsync(stoppingToken);
                    await WaitAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while executing '{ServiceName}', the service is stoping.", nameof(CronWorks));
            }
        }

        private async Task UpdateAsync(CancellationToken stoppingToken)
        {
            var referenceTime = DateTime.UtcNow;

            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            var tasks = _serviceProvider.GetServices<IBackgroundTask>();

            CleanSchedulers();

            var backgroundTaskSettingsProvider = _serviceProvider.GetRequiredService<IBackgroundTaskSettingsProvider>();

            foreach (var task in tasks)
            {
                var taskName = task.GetTaskName();
                if (!_schedulers.TryGetValue(taskName, out var scheduler))
                {
                    _schedulers[taskName] = scheduler = new BackgroundTaskScheduler(taskName, referenceTime);
                }

                BackgroundTaskSettings settings = null;

                settings = await backgroundTaskSettingsProvider.GetSettingsAsync(task);
                scheduler.Settings = settings ?? task.GetDefaultSettings();
            }

            void CleanSchedulers()
            {
                var validKeys = tasks.Select(task => task.GetTaskName()).ToArray();

                var keys = _schedulers.Select(kv => kv.Key).ToArray();

                foreach (var key in keys)
                {
                    if (!validKeys.Contains(key))
                    {
                        _schedulers.TryRemove(key, out var scheduler);
                    }
                }
            }
        }

        private async Task RunAsync(CancellationToken stoppingToken)
        {
            var schedulers = GetSchedulerToRun();

            foreach (var scheduler in schedulers)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                var task = _serviceProvider.GetServices<IBackgroundTask>().GetTaskByName(scheduler.Name);
                if (task == null)
                {
                    return;
                }

                scheduler.Run();

                await task.DoWorkAsync(_serviceProvider, stoppingToken);
            }

            IEnumerable<BackgroundTaskScheduler> GetSchedulerToRun()
                => _schedulers.Where(s => s.Value.CanRun()).Select(s => s.Value).ToArray();
        }

        private async Task WaitAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(PoolingTime, stoppingToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
