using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new TaskFactory();

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return
                TaskFactory
                    .StartNew(func)
                    .Unwrap()
                    .GetAwaiter()
                    .GetResult();
        }
    }
}