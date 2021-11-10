using System.Net;

namespace AuthoBson.Shared.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessful(this HttpStatusCode statusCode)
        {
            return ((int)statusCode >= 200) && ((int)statusCode <= 299);
        }
    }
}