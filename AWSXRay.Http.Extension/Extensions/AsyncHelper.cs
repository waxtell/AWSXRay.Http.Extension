using System;
using System.Threading;
using System.Threading.Tasks;

namespace AWSXRay.Http.Extension.Extensions
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
        public static void RunSync(Func<Task> func)
        {
            AsyncHelper
                .TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return
                AsyncHelper
                    .TaskFactory
                    .StartNew(func)
                    .Unwrap()
                    .GetAwaiter()
                    .GetResult();
        }
    }
}