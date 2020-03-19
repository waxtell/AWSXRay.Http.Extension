using AWSXRay.Http.Extension;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    public static class HostExtensions
    {
        public static IHost ActivateXRayHttpDiagnosticsLogging(this IHost host)
        {
            return
                host
                    .ActivateService<XRayHttpDiagnosticLogger>();
        }
    }
}
