using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moove20.Samples
{
    public static class TaskRepeat
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



    /*
    //http://blogs.msdn.com/b/pfxteam/archive/2010/11/21/10094564.aspx
    */
    public static class TaskExtensions
    {
        public static Task<TResult> SelectMany<TSource, TCollection, TResult>(
            this Task<TSource> source,
            Func<TSource, Task<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (collectionSelector == null) throw new ArgumentNullException("collectionSelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return source.ContinueWith(t =>
            {
                return collectionSelector(t.Result).
                    ContinueWith(c => resultSelector(t.Result, c.Result), TaskContinuationOptions.NotOnCanceled);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }


        public static Task Then(this Task first, Func<Task> next)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (next == null) throw new ArgumentNullException("next");

            var tcs = new TaskCompletionSource<object>();
            first.ContinueWith(delegate
            {
                if (first.IsFaulted) tcs.TrySetException(first.Exception.InnerExceptions);
                else if (first.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        var t = next();
                        if (t == null) tcs.TrySetCanceled();
                        else t.ContinueWith(delegate
                        {
                            if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
                            else if (t.IsCanceled) tcs.TrySetCanceled();
                            else tcs.TrySetResult(null);
                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public static Task Sequence(params Func<Task>[] actions)
        {
            Task last = null;
            foreach (var action in actions)
            {
                last = (last == null) ? Task.Factory.StartNew(action).Unwrap() : last.Then(action);
            }
            return last;
        }
    
    
    
    
    }





}

