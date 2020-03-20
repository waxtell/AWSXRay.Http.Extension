using System.Collections.Generic;
using System.Linq;
using ConfigurationSection.Inheritance.Extension;

namespace AWSXRay.Http.Extension
{
    [KnownType(typeof(HostInclude), "include")]
    [KnownType(typeof(HostExclude), "exclude")]
    [TypeConverter("type")]
    public abstract class HostBelonging : Belonging
    {
    }

    public class HostExclude : HostBelonging
    {
    }

    public class HostInclude : HostBelonging
    {
        public bool IncludeRequestBody { get; set; } = false;
        public bool IncludeResponseBody { get; set; } = false;
        public bool? Traced { get; set; } = null;

        public List<HeaderBelonging> CaptureRequestHeaders { get; set; } = new List<HeaderBelonging>();
        public List<HeaderBelonging> CaptureResponseHeaders { get; set; } = new List<HeaderBelonging>();

        internal bool ShouldCaptureRequestHeader(string comparator)
        {
            return
                CaptureRequestHeaders
                    .LastOrDefault(x => x.IsMatch(comparator)) is HeaderInclude;
        }

        internal bool ShouldCaptureResponseHeader(string comparator)
        {
            return 
                CaptureResponseHeaders
                    .LastOrDefault(x => x.IsMatch(comparator)) is HeaderInclude;
        }
    }
}
