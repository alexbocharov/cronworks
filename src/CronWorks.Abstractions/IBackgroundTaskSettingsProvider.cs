using System.Threading.Tasks;

namespace CronWorks
{
    public interface IBackgroundTaskSettingsProvider
    {
        Task<BackgroundTaskSettings> GetSettingsAsync(IBackgroundTask task);
    }
}
