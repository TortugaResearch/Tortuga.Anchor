using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor;
using Tortuga.Dragnet;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests
{
    [TestClass]
    public class TaskUtilitiesTests
    {
        [TestMethod]
        public void TaskUtilitiesTests_RunConcurrentlyTest()
        {
            using (var verify = new Verify())
            {
                Sleeper(verify).RunConcurrently();
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_ForEachAsync_ListAction_1()
        {
            using (var verify = new Verify())
            {
                var actions = new List<Func<Task>>();
                actions.Add(() => Sleeper(verify));
                actions.Add(() => Sleeper(verify));
                actions.Add(() => Sleeper(verify));
                actions.Add(() => Sleeper(verify));
                actions.Add(() => Sleeper(verify));

                await actions.ForEachAsync();
            }
        }

#if !WINDOWS_UWP
        [TestMethod]
        public async Task TaskUtilitiesTests_ForEachAsync_ListAction_2()
        {
            using (var verify = new Verify())
            {
                var actions = new List<Func<Task>>();
                actions.Add(() => SleeperManualStart(verify));
                actions.Add(() => SleeperManualStart(verify));
                actions.Add(() => SleeperManualStart(verify));
                actions.Add(() => SleeperManualStart(verify));
                actions.Add(() => SleeperManualStart(verify));

                await actions.ForEachAsync();
            }
        }
#endif

        [TestMethod]
        public async Task TaskUtilitiesTests_ForEachAsync_ListT_1()
        {
            using (var verify = new Verify())
            {
                var items = Enumerable.Range(0, 10).ToList();
                await items.ForEachAsync(i => NumericSleeper(verify, i));
            }
        }

#if !WINDOWS_UWP
        [TestMethod]
        public async Task TaskUtilitiesTests_ForEachAsync_ListT_2()
        {
            using (var verify = new Verify())
            {
                var items = Enumerable.Range(0, 10).ToList();
                await items.ForEachAsync(i => NumericSleeperManualStart(verify, i));
            }
        }
#endif

        static async Task Sleeper(Verify verify)
        {
            verify.WriteLine("Before");
            await Task.Delay(1000);
            verify.WriteLine("After");
        }

        static async Task LongSleeper(Verify verify, CancellationToken token)
        {
            verify.WriteLine("Before");
            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                token.ThrowIfCancellationRequested();
            }
            verify.WriteLine("After");
        }

        static async Task NumericSleeper(Verify verify, int i)
        {
            verify.WriteLine("Before " + i);
            await Task.Delay(1000);
            verify.WriteLine("After " + i);
        }

#if !WINDOWS_UWP
        static Task SleeperManualStart(Verify verify)
        {
            return new Task(() =>
            {
                verify.WriteLine("Before");
                Thread.Sleep(1000);
                verify.WriteLine("After");
            });
        }

        static Task NumericSleeperManualStart(Verify verify, int i)
        {
            return new Task(() =>
            {
                verify.WriteLine("Before " + i);
                Thread.Sleep(1000);
                verify.WriteLine("After " + i);
            });
        }

        [TestMethod]
        public void TaskUtilitiesTests_RunConcurrently()
        {
            using (var verify = new Verify())
            {
                var task = new Task(() => Thread.Sleep(100));
                task.RunConcurrently();
            }
        }
#endif

        [TestMethod]
        public void TaskUtilitiesTests_WaitForCancelTest1()
        {
            using (var verify = new Verify())
            {
                var task = Sleeper(verify);
                task.RunConcurrently();

                var result = task.WaitForCompleteOrCancel();

                verify.AreEqual(TaskStatus.RanToCompletion, task.Status, "The task should have been completed");
                verify.IsTrue(result, "Assertion should be true because the task was completed");
            }
        }

        [TestMethod]
        public void TaskUtilitiesTests_WaitForCancelTest2()
        {
            using (var verify = new Verify())
            {
                var cts = new CancellationTokenSource(500);
                var task = LongSleeper(verify, cts.Token);
                task.RunConcurrently();

                var result = task.WaitForCompleteOrCancel();

                verify.AreEqual(TaskStatus.Canceled, task.Status, "The task should have been canceled");
                verify.IsFalse(result, "Assertion should be false because the task was canceled");
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_WhenAnyCancelTest1()
        {
            using (var verify = new Verify())
            {
                var tcs1 = new TaskCompletionSource<object>();
                var tcs2 = new TaskCompletionSource<object>();
                var task1 = tcs1.Task;
                var task2 = tcs2.Task;
                var task3 = Task.Delay(TimeSpan.FromMilliseconds(100));
                var list = new List<Task>() { task1, task2, task3 };
                using (var cs = new CancellationTokenSource())
                {
                    var ct = cs.Token;

                    await list.WhenAny(ct);
                }
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_WhenAnyCancelTest2()
        {
            using (var verify = new Verify())
            {
                try
                {
                    var tcs1 = new TaskCompletionSource<object>();
                    var tcs2 = new TaskCompletionSource<object>();
                    var task1 = tcs1.Task;
                    var task2 = tcs2.Task;
                    var list = new List<Task>() { task1, task2 };
                    using (var cs = new CancellationTokenSource(100))
                    {
                        var ct = cs.Token;

                        await list.WhenAny(ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    //Sucess
                }

            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_WhenAllCancelTest1()
        {
            using (var verify = new Verify())
            {
                var task1 = Task.Delay(TimeSpan.FromMilliseconds(100));
                var task2 = Task.Delay(TimeSpan.FromMilliseconds(125));
                var task3 = Task.Delay(TimeSpan.FromMilliseconds(150));
                var list = new List<Task>() { task1, task2, task3 };
                using (var cs = new CancellationTokenSource())
                {
                    var ct = cs.Token;

                    await list.WhenAll(ct);
                }
                verify.IsTrueForAll(list, t => t.IsCompleted, "All tasks should have been completed.");
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_WhenAllCancelTest2()
        {

            using (var verify = new Verify())
            {
                try
                {
                    var tcs1 = new TaskCompletionSource<object>();
                    var tcs2 = new TaskCompletionSource<object>();
                    var task1 = tcs1.Task;
                    var task2 = tcs2.Task;
                    var list = new List<Task>() { task1, task2 };
                    using (var cs = new CancellationTokenSource(100))
                    {
                        var ct = cs.Token;

                        await list.WhenAll(ct);
                    }
                    verify.Fail("Error expected");
                }
                catch (OperationCanceledException)
                {
                    //Sucess
                }
            }
        }


        //[TestMethod]
        //public void TaskUtilitiesTests_TimerMemoryTest()
        //{
        //    var counter = 0;
        //    Func<Timer> Foo = () => new Timer((a) => { counter += 1; }, null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(int.MaxValue));
        //    var weak = new WeakReference(Foo());
        //    Memory.CycleGC();
        //    Assert.IsFalse(weak.IsAlive);

        //    Thread.Sleep(TimeSpan.FromSeconds(5));

        //    Assert.IsTrue(counter == 1);

        //}

        [TestMethod]
        public void TaskUtilitiesTests_AutoCancelingTaskTest1()
        {
            using (var verify = new Verify())
            {
                var timer = Stopwatch.StartNew();
                var t = TaskUtilities.AutoCancelingTask(TimeSpan.FromSeconds(1));
                Memory.CycleGC();
                var result = t.WaitForCompleteOrCancel();
                timer.Stop();
                verify.IsFalse(result, "WaitForCompleteOrCancel should have returned false");
                verify.IsTrue(t.IsCanceled, "Task should have been canceled.");
                verify.AreEqual(1.0, timer.Elapsed.TotalSeconds, 0.05, "Elapsed time was incorrect.");
            }
        }

        [TestMethod]
        public void TaskUtilitiesTests_AutoCancelingTaskTest2()
        {
            using (var verify = new Verify())
            {
                var timer = Stopwatch.StartNew();
                var t = TaskUtilities.AutoCancelingTask(1000);
                Memory.CycleGC();
                var result = t.WaitForCompleteOrCancel();
                timer.Stop();
                verify.IsFalse(result, "WaitForCompleteOrCancel should have returned false");
                verify.IsTrue(t.IsCanceled, "Task should have been canceled.");
                verify.AreEqual(1.0, timer.Elapsed.TotalSeconds, 0.05, "Elapsed time was incorrect.");
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_WhenAllTest1()
        {
            using (var verify = new Verify())
            {
                var list = new List<Task> { Sleeper(verify), Sleeper(verify), Sleeper(verify) };
                await list.WhenAll();
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_WhenAnyTest1()
        {
            using (var verify = new Verify())
            {
                var list = new List<Task> { Sleeper(verify), Sleeper(verify), Sleeper(verify) };
                await list.WhenAny();
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_AutoCompletingTaskTest1()
        {
            using (var verify = new Verify())
            {
                var timer = Stopwatch.StartNew();
                var task = TaskUtilities.AutoCompletingTask(123, TimeSpan.FromSeconds(1));
                Memory.CycleGC();
                var result = await task;
                timer.Stop();
                verify.AreEqual(123, result, "Task result was wrong");
                verify.AreEqual(1.0, timer.Elapsed.TotalSeconds, 0.05, "Delay was incorrect");
            }
        }

        [TestMethod]
        public async Task TaskUtilitiesTests_AutoCompletingTaskTest2()
        {
            using (var verify = new Verify())
            {
                var timer = Stopwatch.StartNew();
                var task = TaskUtilities.AutoCompletingTask(123, 1000);
                Memory.CycleGC();
                var result = await task;
                timer.Stop();
                verify.AreEqual(123, result, "Task result was wrong");
                verify.AreEqual(1.0, timer.Elapsed.TotalSeconds, 0.05, "Delay was incorrect");
            }
        }

    }

}
