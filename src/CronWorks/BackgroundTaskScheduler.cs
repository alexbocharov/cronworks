using System;
using NCrontab;

namespace CronWorks
{
    public class BackgroundTaskScheduler
    {
        public BackgroundTaskScheduler(string name, DateTime referenceTime)
        {
            Name = name;
            ReferenceTime = referenceTime;
            Settings = new BackgroundTaskSettings { Name = name };
            State = new BackgroundTaskState { Name = name };
        }

        public string Name { get; }
        public DateTime ReferenceTime { get; set; }
        public BackgroundTaskSettings Settings { get; set; }
        public BackgroundTaskState State { get; set; }

        public bool CanRun()
        {
            var nextStartTime = CrontabSchedule.Parse(Settings.Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true }).GetNextOccurrence(ReferenceTime);

            if (DateTime.UtcNow >= nextStartTime)
            {
                if (Settings.Enable)
                {
                    return true;
                }

                ReferenceTime = DateTime.UtcNow;
            }

            return false;
        }

        public void Run()
        {
            State.LastStartTime = ReferenceTime = DateTime.UtcNow;
        }
    }
}
