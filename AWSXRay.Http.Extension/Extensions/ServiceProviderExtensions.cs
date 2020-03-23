using Microsoft.Extensions.DependencyInjection;

namespace AWSXRay.Http.Extension.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static ServiceProvider ActivateXRayHttpDiagnosticsLogging(this ServiceProvider provider)
        {
            provider.GetService<XRayHttpDiagnosticLogger>();

            return provider;
        }
    }
}
