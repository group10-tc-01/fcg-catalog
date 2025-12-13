using System.Net;

namespace FCG.Catalog.Domain.Exception
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
        public UnauthorizedException(string message, System.Exception innerException) : base(HttpStatusCode.Unauthorized, message, innerException) { }
    }
}
