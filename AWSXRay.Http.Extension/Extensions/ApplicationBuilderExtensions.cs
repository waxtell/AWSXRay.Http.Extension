using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AWSXRay.Http.Extension.Extensions
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
