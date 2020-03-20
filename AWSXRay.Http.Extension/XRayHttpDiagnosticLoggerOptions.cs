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
            var lastMatch = CaptureHosts.LastOrDefault(x => x.IsMatch(comparator));

            include = lastMatch as Include;

            return lastMatch is Include;
        }
    }
}
