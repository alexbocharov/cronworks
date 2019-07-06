using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CronWorks
{
    public class BackgroundTaskOptionsSetup : IConfigureOptions<BackgroundTaskOptions>
    {
        private readonly IConfiguration _configuration;

        public BackgroundTaskOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(BackgroundTaskOptions options)
        {
            var taskSettings = _configuration.GetSection("Scheduler:Tasks")?.Get<ICollection<BackgroundTaskSettings>>();
            if (taskSettings != null)
            {
                options.TaskSettings = taskSettings.ToDictionary(x => x.Name);
            }

        }
    }
}
