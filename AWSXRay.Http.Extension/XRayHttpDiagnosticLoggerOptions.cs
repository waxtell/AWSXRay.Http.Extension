using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AWSXRay.Http.Extension.Tests")]

namespace AWSXRay.Http.Extension
{
    public class XRayHttpDiagnosticLoggerOptions
    {
        public List<HostBelonging> CaptureHosts { get; set; } = new List<HostBelonging>();

        internal bool ShouldCaptureHost(string comparator, out HostInclude include)
        {
            var lastMatch = CaptureHosts.LastOrDefault(x => x.IsMatch(comparator));

            include = lastMatch as HostInclude;

            return lastMatch is HostInclude;
        }
    }
}
