using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CronWorks
{
    public static class CronWorksServiceCollectionExtensions
    {
        public static IServiceCollection AddCronWorks(this IServiceCollection services)
        {
            services.AddHostedService<CronWorks>();
            services.AddSingleton<IConfigureOptions<BackgroundTaskOptions>, BackgroundTaskOptionsSetup>();
            services.AddSingleton<IBackgroundTaskSettingsProvider, BackgroundTaskSettingsProvider>();

            return services;
        }
    }
}
