using System.Linq;

namespace AWSXRay.Http.Extension.Extensions
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
