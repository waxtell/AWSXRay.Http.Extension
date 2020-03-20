using ConfigurationSection.Inheritance.Extension;

namespace AWSXRay.Http.Extension
{
    [KnownType(typeof(HeaderInclude), "include")]
    [KnownType(typeof(HeaderExclude), "exclude")]
    [TypeConverter("type")]
    public abstract class HeaderBelonging : Belonging
    {
    }

    public class HeaderExclude : HeaderBelonging
    {
    }

    public class HeaderInclude : HeaderBelonging
    {
    }
}
