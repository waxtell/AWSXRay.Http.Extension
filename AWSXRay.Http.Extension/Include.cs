namespace AWSXRay.Http.Extension
{
    public class Include : Belonging
    {
        public bool IncludeRequestBody { get; set; } = false;
        public bool IncludeResponseBody { get; set; } = false;

        public bool? Traced { get; set; } = null;
    }
}
