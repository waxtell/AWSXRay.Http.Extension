using System;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    internal static class ObjectExtensions
    {
        public static T With<T>(this T obj, Action<T> action)
        {
            action(obj);

            return obj;
        }
    }
}
