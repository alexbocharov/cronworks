using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace CronWorks
{
    public class BackgroundTaskSettingsProvider : IBackgroundTaskSettingsProvider
    {
        private readonly IOptions<BackgroundTaskOptions> _options;

        public BackgroundTaskSettingsProvider(IOptions<BackgroundTaskOptions> options)
        {
            _options = options;
        }

        public Task<BackgroundTaskSettings> GetSettingsAsync(IBackgroundTask task)
        {
            if (_options.Value.TaskSettings.TryGetValue(task.GetTaskName(), out var settings))
            {
                return Task.FromResult(settings);
            }

            return null;
        }
    }
}
