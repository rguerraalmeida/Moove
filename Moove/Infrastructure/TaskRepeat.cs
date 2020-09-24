using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class TaskRepeat
    {
        public static Task Interval(TimeSpan pollInterval, Action action, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                while (!token.WaitCancellationRequested(pollInterval))
                {
                    action();
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static Task Interval(TimeSpan pollInterval, Action action, CancellationToken token, TaskScheduler scheduler)
        {
            return Task.Factory.StartNew(() =>
            {
                while (!token.WaitCancellationRequested(pollInterval))
                {
                    action();
                }
            }, token, TaskCreationOptions.LongRunning, scheduler);
        }
    }
}
