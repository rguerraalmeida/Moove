using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MooveUI.Extensions
{
    public static class TaskRepeatExtension
    {
        public static Task Interval(
            TimeSpan pollInterval,
            Action action,
            CancellationToken token)
        {
            // We don't use Observable.Interval:
            // If we block, the values start bunching up behind each other.
            return Task.Factory.StartNew(
                () =>
                {
                    for (; ; )
                    {
                        action();

                        if (token.WaitCancellationRequested(pollInterval))
                            break;

                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static Task Interval(
            TimeSpan pollInterval,
            Action action,
            CancellationToken token,
            TaskScheduler scheduler)
        {
            // We don't use Observable.Interval:
            // If we block, the values start bunching up behind each other.
            return Task.Factory.StartNew(
                () =>
                {
                    for (; ; )
                    {
                        action();

                        if (token.WaitCancellationRequested(pollInterval))
                            break;


                    }
                }, token, TaskCreationOptions.LongRunning, scheduler);
        }
    }

    public static class CancellationTokenExtensions
    {
        public static bool WaitCancellationRequested(
            this CancellationToken token,
            TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}
