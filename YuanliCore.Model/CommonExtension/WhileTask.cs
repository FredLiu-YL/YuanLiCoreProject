using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YuanliCore.Model.CommonExtension
{
    public class WhileTask : IAsyncDisposable, IDisposable
    {
        private readonly CancellationTokenSource cts;
        private readonly Task task;

        private WhileTask(Task task, CancellationTokenSource cts)
        {
            this.task = task;
            this.cts = cts;
        }

        /// <summary>
        /// 取得這個 Task 的工作狀態。
        /// </summary>
        public TaskStatus Status => task.Status;

        /// <summary>
        /// 取得這個 Task 是否已經完成。
        /// </summary>
        public bool IsCompleted => task.IsCompleted;

        /// <summary>
        /// 取得這個 Task 是否因為處理的例外狀況才完成。
        /// </summary>
        public bool IsFaulted => task.IsFaulted;

        /// <summary>
        /// 取得導致 Task 不當結束的例外狀況。
        /// </summary>
        public AggregateException Exception => task.Exception;

        public static implicit operator Task(WhileTask whileTask)
            => whileTask.task;

        public static WhileTask CompletedWhileTask => new WhileTask(Task.CompletedTask, null);

        public static WhileTask Run(Action<CancellationToken> action, int delayTime)
        {
            var cts = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    do
                    {
                        action(cts.Token);
                        if (delayTime > 0) Thread.Sleep(delayTime);
                        if (cts.IsCancellationRequested) break;
                    }
                    while (!cts.IsCancellationRequested);
                }
                catch (OperationCanceledException)
                {
                    // do nothing...
                }
            });

            return new WhileTask(task, cts);
        }

        public static WhileTask Run(Action action, int delayTime)
            => WhileTask.Run(token => { action(); }, delayTime);

        //public static WhileTask Run(Action action, int delayTime)
        //{
        //    var cts = new CancellationTokenSource();
        //    var task = Task.Run(async () =>
        //    {
        //        do
        //        {
        //            action();
        //            if (delayTime > 0)
        //                await Task.Delay(delayTime, cts.Token);
        //            if (cts.IsCancellationRequested) break;
        //        }
        //        while (!cts.IsCancellationRequested);
        //    });

        //    return new WhileTask(task, cts);
        //}

        public static WhileTask Run(Func<bool> condiction, int delayTime)
        {
            var cts = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                do
                {
                    if (condiction()) break;
                    if (delayTime > 0) Thread.Sleep(delayTime);
                    if (cts.IsCancellationRequested) break;
                }
                while (!cts.IsCancellationRequested);
            });

            return new WhileTask(task, cts);
        }

        public static WhileTask Run(Func<Task> func, int delayTime)
        {
            var cts = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                do
                {
                    await func();
                    if (delayTime > 0) Thread.Sleep(delayTime);
                    if (cts.IsCancellationRequested) break;
                }
                while (!cts.IsCancellationRequested);
            });

            return new WhileTask(task, cts);
        }

        public static WhileTask Run(Func<Task<bool>> condiction, int delayTime)
        {
            var cts = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                do
                {
                    if (await condiction()) break;
                    if (delayTime > 0) Thread.Sleep(delayTime);
                    if (cts.IsCancellationRequested) break;
                }
                while (!cts.IsCancellationRequested);
            });

            return new WhileTask(task, cts);
        }

        public void Dispose() => Task.Run(DisposeAsync).Wait();

        public async Task DisposeAsync()
        {
            if (task.IsCompleted) return;
            cts.Cancel();
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                // 不攔截取消的錯誤。
            }
        }

        public TaskAwaiter GetAwaiter() => task.GetAwaiter();
    }

    public interface IAsyncDisposable
    {
        Task DisposeAsync();
    }
}
