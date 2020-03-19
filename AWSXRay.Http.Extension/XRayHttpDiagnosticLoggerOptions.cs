using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AWSXRay.Http.Extension.Tests")]

namespace AWSXRay.Http.Extension
{
    public class XRayHttpDiagnosticLoggerOptions
    {
        public List<Belonging> CaptureHosts { get; set; } = new List<Belonging>();

        internal bool ShouldCaptureDetails(string comparator, out Include include)
        {
            var exclude = CaptureHosts
                            ?.OfType<Exclude>()
                            .Any(x => x.IsMatch(comparator));

            include = CaptureHosts
                            ?.OfType<Include>()
                            .First(x => x.IsMatch(comparator));

            return 
            (
                include != null && 
                (!exclude.HasValue || !exclude.Value)
            );
        }
    }
}
