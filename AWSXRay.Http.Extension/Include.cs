namespace AWSXRay.Http.Extension
{
    public class Include : Belonging
    {
        public bool IncludeRequestBody { get; set; }
        public bool IncludeResponseBody { get; set; }
    }
}
