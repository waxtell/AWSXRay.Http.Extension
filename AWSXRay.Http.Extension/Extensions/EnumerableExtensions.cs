using System.Linq;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    internal static class EnumerableExtensions
    {
        public static bool In<T>(this T source, params T[] collection)
        {
            return
                collection != null &&
                collection.Contains(source);
        }
	}
}
