using ConfigurationSection.Inheritance.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AWSXRay.Http.Extension.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpXRayTracing(this IServiceCollection collection)
        {
            return
                AddHttpXRayTracing(collection, new XRayHttpDiagnosticLoggerOptions());
        }

        public static IServiceCollection AddHttpXRayTracing(this IServiceCollection collection, IConfiguration config, string configKey = nameof(XRayHttpDiagnosticLoggerOptions))
        {
            return
                AddHttpXRayTracing
                (
                    collection, 
                    config
                        .GetSection(configKey)
                        .GetEx<XRayHttpDiagnosticLoggerOptions>()
                );
        }

        public static IServiceCollection AddHttpXRayTracing(this IServiceCollection collection, XRayHttpDiagnosticLoggerOptions options)
        {
            return
                collection
                    .AddSingleton(options)
                    .AddSingleton<XRayHttpDiagnosticLogger>();
        }
    }
}
