﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CronWorks
{
    public static class BackgroundTaskExtensions
    {
        public static BackgroundTaskSettings GetDefaultSettings(this IBackgroundTask task)
        {
            var type = task.GetType();

            var attribute = type.GetCustomAttribute<BackgroundTaskAttribute>();
            if (attribute != null)
            {
                return new BackgroundTaskSettings
                {
                    Name = type.FullName,
                    Enable = attribute.Enable,
                    Schedule = attribute.Schedule,
                    Description = attribute.Description
                };
            }

            return new BackgroundTaskSettings { Name = type.FullName };
        }

        public static string GetTaskName(this IBackgroundTask task)
            => task.GetType().FullName;

        public static IBackgroundTask GetTaskByName(this IEnumerable<IBackgroundTask> tasks, string name)
            => tasks.LastOrDefault(t => t.GetTaskName() == name);
    }
}
