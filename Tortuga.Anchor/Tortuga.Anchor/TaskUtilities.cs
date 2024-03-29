﻿using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Anchor;

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
		return AsTask<object>(cancellationToken);
	}

	/// <summary>Allows using a Cancellation Token as if it were a task.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that can be canceled, but never completed.</returns>
	public static Task<T> AsTask<T>(this CancellationToken cancellationToken)
	{
		var tcs = new TaskCompletionSource<T>();
		cancellationToken.Register(() => tcs.TrySetCanceled(), false);
		return tcs.Task;
	}

	/// <summary>
	/// Creates a task that automatically cancels after a given delay.
	/// </summary>
	/// <typeparam name="T">Task type</typeparam>
	/// <param name="delay">The delay before the task is canceled.</param>
	/// <returns>Task that will be canceled.</returns>
	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
	public static Task<T> AutoCancelingTask<T>(TimeSpan delay)
	{
		if (delay.TotalMilliseconds < 0)
			throw new ArgumentOutOfRangeException(nameof(delay), delay, $"{nameof(delay)} cannot be less than 0");

		var tcs = new TaskCompletionSource<T>();
		var t = new System.Timers.Timer(delay.TotalMilliseconds);
		t.AutoReset = false;
		t.Elapsed += (source, e) =>
		{
			t.Dispose();
			tcs.SetCanceled();
		};
		t.Start();
		return tcs.Task;
	}

	/// <summary>
	/// Creates a task that automatically cancels after a given delay.
	/// </summary>
	/// <param name="delay">The delay before the task is canceled.</param>
	/// <returns>Task that will be canceled.</returns>
	public static Task AutoCancelingTask(TimeSpan delay)
	{
		return AutoCancelingTask<object>(delay);
	}

	/// <summary>
	/// Creates a task that automatically cancels after a given delay.
	/// </summary>
	/// <typeparam name="T">Task type</typeparam>
	/// <param name="delay">The delay, in milliseconds, before the task is canceled.</param>
	/// <returns>Task that will be canceled.</returns>
	/// <remarks>Use Task.Delay if a result isn't needed.</remarks>
	public static Task<T> AutoCancelingTask<T>(int delay)
	{
		return AutoCancelingTask<T>(TimeSpan.FromMilliseconds(delay));
	}

	/// <summary>
	/// Creates a task that automatically cancels after a given delay.
	/// </summary>
	/// <param name="delay">The delay, in milliseconds, before the task is canceled.</param>
	/// <returns>Task that will be canceled.</returns>
	public static Task AutoCancelingTask(int delay)
	{
		return AutoCancelingTask<object>(delay);
	}

	/// <summary>
	/// Automatically cancels a CancellationToken after the indicated amount of time.
	/// </summary>
	/// <param name="delay">The delay before the token is canceled.</param>
	/// <returns>CancellationToken.</returns>
	/// <exception cref="ArgumentOutOfRangeException">delay</exception>
	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
	public static CancellationToken AutoCancelingToken(TimeSpan delay)
	{
		if (delay.TotalMilliseconds < 0)
			throw new ArgumentOutOfRangeException(nameof(delay), delay, $"{nameof(delay)} cannot be less than 0");

		var cts = new CancellationTokenSource(delay);
		var result = cts.Token;
		result.Register(() => cts.Dispose());
		return result;
	}

	/// <summary>
	/// Automatically cancels a CancellationToken after the indicated amount of time in milliseconds.
	/// </summary>
	/// <param name="millisecondsDelay">The delay in milliseconds before the token is canceled.</param>
	/// <returns>CancellationToken.</returns>
	/// <exception cref="ArgumentOutOfRangeException">delay</exception>
	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
	public static CancellationToken AutoCancelingToken(int millisecondsDelay)
	{
		if (millisecondsDelay < 0)
			throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), millisecondsDelay, $"{nameof(millisecondsDelay)} cannot be less than 0");

		var cts = new CancellationTokenSource(millisecondsDelay);
		var result = cts.Token;
		result.Register(() => cts.Dispose());
		return result;
	}

	/// <summary>
	/// Creates a task that automatically completes after a given delay.
	/// </summary>
	/// <typeparam name="T">Task type</typeparam>
	/// <param name="result">The result to be given to the task.</param>
	/// <param name="delay">The delay before the task is completed.</param>
	/// <returns>Task that will be completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException">delay</exception>
	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
	public static Task<T> AutoCompletingTask<T>(T result, TimeSpan delay)
	{
		if (delay.TotalMilliseconds < 0)
			throw new ArgumentOutOfRangeException(nameof(delay), delay, $"{nameof(delay)} cannot be less than 0");

		var tcs = new TaskCompletionSource<T>();
		var t = new System.Timers.Timer(delay.TotalMilliseconds);
		t.AutoReset = false;
		t.Elapsed += (source, e) =>
		{
			t.Dispose();
			tcs.SetResult(result);
		};
		t.Start();
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
	public static Task<T> AutoCompletingTask<T>(T result, int delay)
	{
		return AutoCompletingTask(result, TimeSpan.FromMilliseconds(delay));
	}

	/// <summary>
	/// Creates a cancellable task that completes after a specified time interval. This will not throw an exception if the cancellationToken is triggered or disposed.
	/// </summary>
	/// <param name="delay">The time span to wait before completing the returned task, or TimeSpan.FromMilliseconds(-1) to wait indefinitely.</param>
	/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
	/// <returns>A task that represents the time delay.</returns>
	/// <exception cref="ArgumentOutOfRangeException">The millisecondsDelay argument is less than -1.</exception>
	public static async Task DelaySafe(TimeSpan delay, CancellationToken cancellationToken)
	{
		try
		{
			await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
		}
		catch (ObjectDisposedException) { }
		catch (TaskCanceledException) { }
	}

	/// <summary>
	/// Creates a cancellable task that completes after a specified number of milliseconds. This will not throw an exception if the cancellationToken is triggered or disposed.
	/// </summary>
	/// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned task, or -1 to wait indefinitely.
	/// </param>
	/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
	/// <returns>A task that represents the time delay.</returns>
	/// <exception cref="ArgumentOutOfRangeException">The millisecondsDelay argument is less than -1.</exception>
	public static async Task DelaySafe(int millisecondsDelay, CancellationToken cancellationToken)
	{
		try
		{
			await Task.Delay(millisecondsDelay, cancellationToken).ConfigureAwait(false);
		}
		catch (ObjectDisposedException) { }
		catch (TaskCanceledException) { }
	}

	/// <summary>
	/// Runs each action in parallel.
	/// </summary>
	/// <param name="actions">The actions.</param>
	/// <returns>A Task that is completed when all actions are complete</returns>
	/// <exception cref="ArgumentNullException">actions</exception>
	/// <exception cref="ArgumentException">actions</exception>
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

		var tcs = new TaskCompletionSource<object?>();

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
	/// <returns>A Task that is completed when all actions are complete</returns>
	/// <exception cref="ArgumentNullException">
	/// list
	/// or
	/// action
	/// </exception>
	public static Task ForEachAsync<T>(this IReadOnlyCollection<T> list, Func<T, Task> action)
	{
		if (list == null)
			throw new ArgumentNullException(nameof(list), $"{nameof(list)} is null.");
		if (action == null)
			throw new ArgumentNullException(nameof(action), $"{nameof(action)} is null.");

		var tcs = new TaskCompletionSource<object?>();

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
	/// <exception cref="ArgumentNullException">task</exception>
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
		catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions.Single() is OperationCanceledException)
		{
			return false;
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

		await Task.WhenAny(Task.WhenAll(tasks), cancellationToken.AsTask()).ConfigureAwait(false);
		cancellationToken.ThrowIfCancellationRequested();
	}

	/// <summary>
	/// Task.WhenAll as an extension method.
	/// </summary>
	/// <param name="tasks">The tasks to wait for.</param>
	/// <returns>Task.</returns>
	/// <exception cref="ArgumentNullException">tasks</exception>
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

		await Task.WhenAny(tasks.Concat(cancellationToken.AsTask())).ConfigureAwait(false);
		cancellationToken.ThrowIfCancellationRequested();
	}

	/// <summary>
	/// A version of Task.WhenAll that can be canceled.
	/// </summary>
	/// <param name="tasks">The tasks to wait for.</param>
	/// <returns>Task.</returns>
	/// <exception cref="ArgumentNullException">tasks</exception>
	public static Task WhenAny(this IEnumerable<Task> tasks)
	{
		if (tasks == null)
			throw new ArgumentNullException(nameof(tasks), $"{nameof(tasks)} is null.");

		return Task.WhenAny(tasks);
	}
}
