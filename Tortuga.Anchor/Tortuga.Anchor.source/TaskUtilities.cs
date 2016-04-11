using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Extension methods for working with Tasks
    /// </summary>
    public static class TaskUtilities
    {
        /// <summary>
        /// Allows using a Cancellation Token as if it were a task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that can be canceled, but never completed.</returns>
        public static Task AsTask(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();
            cancellationToken.Register(() => tcs.TrySetCanceled(), false);
            return tcs.Task;
        }

#if !Timer_Missing
        /// <summary>
        /// Creates a task that automatically completes after a given delay.
        /// </summary>
        /// <typeparam name="T">Task type</typeparam>
        /// <param name="delay">The delay before the task is completed.</param>
        /// <returns>Task that will be completed.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Task AutoCancelingTask<T>(TimeSpan delay)
        {
            if (delay.TotalMilliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(delay), delay, $"{nameof(delay)} cannot be less than 0");

            var tcs = new TaskCompletionSource<object>();
            Timer t = null; //What prevents this timer from being prematurely garbage collected?
            t = new Timer(state =>
            {
                t.Dispose();
                tcs.SetCanceled();
            }, null, delay, TimeSpan.FromMilliseconds(int.MaxValue));
            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that automatically completes after a given delay.
        /// </summary>
        /// <param name="delay">The delay before the task is completed.</param>
        /// <returns>Task that will be completed.</returns>
        public static Task AutoCancelingTask(TimeSpan delay)
        {
            return AutoCancelingTask<object>(delay);
        }

        /// <summary>
        /// Creates a task that automatically completes after a given delay.
        /// </summary>
        /// <typeparam name="T">Task type</typeparam>
        /// <param name="delay">The delay, in milliseconds, before the task is completed.</param>
        /// <returns>Task that will be completed.</returns>
        /// <remarks>Use Task.Delay if a result isn't needed.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Task AutoCancelingTask<T>(int delay)
        {
            if (delay < 0)
                throw new ArgumentOutOfRangeException(nameof(delay), delay, $"{nameof(delay)} cannot be less than 0");

            var tcs = new TaskCompletionSource<object>();
            Timer t = null; //What prevents this timer from being prematurely garbage collected?
            t = new Timer(state =>
            {
                t.Dispose();
                tcs.SetCanceled();
            }, null, delay, int.MaxValue);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that automatically completes after a given delay.
        /// </summary>
        /// <param name="delay">The delay, in milliseconds, before the task is completed.</param>
        /// <returns>Task that will be completed.</returns>
        public static Task AutoCancelingTask(int delay)
        {
            return AutoCancelingTask<object>(delay);
        }

        /// <summary>
        /// Creates a task that automatically completes after a given delay.
        /// </summary>
        /// <typeparam name="T">Task type</typeparam>
        /// <param name="result">The result to be given to the task.</param>
        /// <param name="delay">The delay before the task is completed.</param>
        /// <returns>Task that will be completed.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AutoCompleting")]
        public static Task<T> AutoCompletingTask<T>(T result, TimeSpan delay)
        {
            if (delay.TotalMilliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(delay), delay, $"{nameof(delay)} cannot be less than 0");

            var tcs = new TaskCompletionSource<T>();
            Timer t = null; //What prevents this timer from being prematurely garbage collected?
            t = new Timer(state =>
            {
                t.Dispose();
                tcs.SetResult(result);
            }, null, delay, TimeSpan.FromMilliseconds(int.MaxValue));
            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that automatically completes after a given delay.
        /// </summary>
        /// <typeparam name="T">Task type</typeparam>
        /// <param name="result">The result to be given to the task.</param>
        /// <param name="delay">The delay, in milliseconds, before the task is completed.</param>
        /// <returns>Task that will be completed.</returns>
        /// <remarks>Use Task.Delay if a result isn't needed.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AutoCompleting")]
        public static Task<T> AutoCompletingTask<T>(T result, int delay)
        {
            if (delay < 0)
                throw new ArgumentOutOfRangeException(nameof(delay), delay, $"{nameof(delay)} cannot be less than 0");

            var tcs = new TaskCompletionSource<T>();
            Timer t = null; //What prevents this timer from being prematurely garbage collected?
            t = new Timer(state =>
            {
                t.Dispose();
                tcs.SetResult(result);
            }, null, delay, int.MaxValue);
            return tcs.Task;
        }
#endif

        /// <summary>
        /// Runs each action in parallel.
        /// </summary>
        /// <param name="actions">The actions.</param>
        /// <returns>A Task that is completed when all actions are complete</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Task ForEachAsync(this IReadOnlyCollection<Func<Task>> actions)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions), $"{nameof(actions)} is null.");


            var tasks = new Task[actions.Count];
            var i = 0;
            foreach (var action in actions)
            {
                if (action == null)
                    throw new ArgumentException($"{nameof(actions)} cannot contain null items", nameof(actions));
                tasks[i] = action();
                if (tasks[i].Status == TaskStatus.Created)
                    tasks[i].Start();

                i++;
            }

            var tcs = new TaskCompletionSource<object>();

            Task.Run(() =>
            {
                Task.WaitAll(tasks);
                tcs.SetResult(null);
            });

            return tcs.Task;
        }

        /// <summary>
        /// In parallel, performs the same asynchronous action on each element in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list of input values to be passed to the action.</param>
        /// <param name="action">The action to be performed on each element in the list.</param>
        /// <returns>
        /// A Task that is completed when all actions are complete
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Task ForEachAsync<T>(this IReadOnlyCollection<T> list, Func<T, Task> action)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
            if (action == null)
                throw new ArgumentNullException(nameof(action), $"{nameof(action)} is null.");

            var tcs = new TaskCompletionSource<object>();

            Task.Run(() =>
            {
                var tasks = new Task[list.Count];
                var i = 0;
                foreach (var item in list)
                {
                    tasks[i] = action(item);
                    if (tasks[i].Status == TaskStatus.Created)
                        tasks[i].Start();
                    i++;
                }
                Task.WaitAll(tasks);
                tcs.SetResult(null);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Runs the Task in a concurrent thread without waiting for it to complete. This will start the task if it is not already running.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <remarks>This is usually used to avoid warning messages about not waiting for the task to complete.</remarks>
        public static void RunConcurrently(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task), $"{nameof(task)} is null.");

            if (task.Status == TaskStatus.Created)
                task.Start();
        }
        /// <summary>
        /// Wait for the indicated task to be completed or canceled.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns><c>true</c> if the task ran to completion, <c>false</c> if the task was canceled.</returns>
        /// <exception cref="ArgumentNullException">task;task is null.</exception>
        public static bool WaitForCompleteOrCancel(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task), $"{nameof(task)} is null.");

            try
            {
                task.Wait();
                return true;
            }
            catch (AggregateException ex)
            {
                //rethrow the exception if we received anything except OperationCanceledException in the AggregateException
                if (ex.InnerExceptions.Count == 1 && ex.InnerExceptions.Single() is OperationCanceledException)
                    return false;
                else
                    throw;
            }
        }
        /// <summary>
        /// A version of Task.WhenAll that can be canceled.
        /// </summary>
        /// <param name="tasks">The tasks to wait for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task WhenAll(this IEnumerable<Task> tasks, CancellationToken cancellationToken)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks), $"{nameof(tasks)} is null.");

            await Task.WhenAny(Task.WhenAll(tasks), cancellationToken.AsTask());
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Task.WhenAll as an extension method.
        /// </summary>
        /// <param name="tasks">The tasks to wait for.</param>
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks), $"{nameof(tasks)} is null.");

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// A version of Task.WhenAny that can be canceled.
        /// </summary>
        /// <param name="tasks">The tasks to wait for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task WhenAny(this IEnumerable<Task> tasks, CancellationToken cancellationToken)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks), $"{nameof(tasks)} is null.");

            await Task.WhenAny(tasks.Concat(cancellationToken.AsTask()));
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// A version of Task.WhenAll that can be canceled.
        /// </summary>
        /// <param name="tasks">The tasks to wait for.</param>
        public static Task WhenAny(this IEnumerable<Task> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks), $"{nameof(tasks)} is null.");

            return Task.WhenAny(tasks);
        }
    }
}


