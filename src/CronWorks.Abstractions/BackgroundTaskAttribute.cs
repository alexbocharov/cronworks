using System;

namespace CronWorks
{
    public class BackgroundTaskAttribute : Attribute
    {
        public bool Enable { get; set; } = true;
        public string Schedule { get; set; } = "* * * * * *";
        public string Description { get; set; } = string.Empty;
    }
}
