using System;
using System.Threading;
using System.Threading.Tasks;

namespace CronWorks
{
    public interface IBackgroundTask
    {
        Task DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }
}
