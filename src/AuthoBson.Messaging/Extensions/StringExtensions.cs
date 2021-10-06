using System;

namespace AuthoBson.Messaging.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string lhs, string rhs)
        {
            return lhs.Equals(rhs, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public static bool ContainsIgnoreCase(this string fullString, string substring)
        {
            if (! string.IsNullOrEmpty(substring) && ! string.IsNullOrEmpty(fullString))
            {
                return fullString.ToLower().Contains(substring.ToLower());
            }

            return true;
        }
    }
}