using System;
using System.Collections.Generic;

namespace CronWorks
{
    public class BackgroundTaskOptions
    {
        public IDictionary<string, BackgroundTaskSettings> TaskSettings { get; set; } = new Dictionary<string, BackgroundTaskSettings>();
    }
}
