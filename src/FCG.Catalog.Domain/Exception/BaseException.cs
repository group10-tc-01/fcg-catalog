using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FCG.Catalog.Domain.Exception
{
    [ExcludeFromCodeCoverage]
    public class BaseException : System.Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected BaseException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        protected BaseException(HttpStatusCode statusCode, string message, System.Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
