using System;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    internal static class ObjectExtensions
    {
        public static T With<T>(this T obj, Action<T> action)
        {
            try
            {
                action(obj);
            }
            catch (Exception e)
            {
                // Don't allow any exceptions to surface to the user code!
                Console.WriteLine(e.Message);
            }

            return obj;
        }
    }
}
