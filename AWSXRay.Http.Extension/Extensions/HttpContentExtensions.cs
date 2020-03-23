using System;
using System.Net.Http;
using System.Net.Mime;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    internal static class HttpContentExtensions
    {
        public static bool IsJson(this HttpContent content)
        {
            return
                content?.Headers.ContentType != null &&
                content.Headers.ContentType.MediaType.Equals("application/json",StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHumanReadable(this HttpContent content)
        {
            return
                content?.Headers.ContentType != null && 
                content.Headers.ContentType.MediaType.In
                (
                    MediaTypeNames.Text.Plain,
                    MediaTypeNames.Text.Xml,
                    MediaTypeNames.Text.Html,
                    "text/namevalue",
                    MediaTypeNames.Application.Soap,
                    "application/xml"
                );
        }

        public static object ToObject(this HttpContent content)
        {
            object contentObject = "Content not human readable.";

            if (content.IsJson())
            {
                contentObject = ThirdParty
                                    .LitJson
                                    .JsonMapper
                                    .ToObject(AsyncHelper.RunSync(content.ReadAsStringAsync));
            }
            else if (content.IsHumanReadable())
            {
                contentObject = AsyncHelper.RunSync(content.ReadAsStringAsync);
            }

            return contentObject;
        }
    }
}
