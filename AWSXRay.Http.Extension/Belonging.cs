using System;
using System.Text.RegularExpressions;

namespace AWSXRay.Http.Extension
{
    public abstract class Belonging
    {
        public bool IsRegEx { get; set; } = false;
        public string Expression { get; set; }

        internal bool IsMatch(string comparator)
        {
            if (IsRegEx)
            {
                var regex = new Regex(Expression);

                return regex.IsMatch(comparator);
            }

            return 
                Expression
                    .Equals(comparator, StringComparison.OrdinalIgnoreCase);
        }
    }
}
