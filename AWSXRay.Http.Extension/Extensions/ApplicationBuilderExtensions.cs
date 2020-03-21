using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace AWSXRay.Http.Extension
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ActivateXRayHttpDiagnosticsLogging(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<XRayHttpDiagnosticLogger>();

            return app;
        }
    }
}
