using Xunit;

namespace AWSXRay.Http.Extension.Tests
{
    public class BelongingTests
    {
        [Fact]
        public void IncludedAndExcludedMatchCaptureIsFalse()
        {
            var options = new XRayHttpDiagnosticLoggerOptions();
            options.CaptureHosts.Add(new HostInclude { Expression = "google.com", IsRegEx = false });
            options.CaptureHosts.Add(new HostExclude { Expression = "google.com", IsRegEx = false });

            Assert.False(options.ShouldCaptureHost("google.com", out _));
        }

        [Fact]
        public void HostRegExMatchIncludedCaptureIsTrue()
        {
            var options = new XRayHttpDiagnosticLoggerOptions();
            options.CaptureHosts.Add(new HostInclude { Expression = ".*", IsRegEx = true });

            Assert.True(options.ShouldCaptureHost("google.com", out _));
        }

        [Fact]
        public void HostRegExMatchExcludedCaptureIsTrue()
        {
            var options = new XRayHttpDiagnosticLoggerOptions();
            options.CaptureHosts.Add(new HostExclude { Expression = ".*", IsRegEx = true });

            Assert.False(options.ShouldCaptureHost("google.com", out _));
        }
    }
}
