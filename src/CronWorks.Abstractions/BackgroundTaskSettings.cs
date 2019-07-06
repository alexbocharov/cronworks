namespace CronWorks
{
    public class BackgroundTaskSettings
    {
        public string Name { get; set; } = string.Empty;
        public bool Enable { get; set; } = true;
        public string Schedule { get; set; } = "* * * * * *";
        public string Description { get; set; } = string.Empty;
    }
}
