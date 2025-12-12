using System.Net;

namespace FCG.Catalog.Domain.Exception
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
        public NotFoundException(string message, System.Exception innerException) : base(HttpStatusCode.NotFound, message, innerException) { }
    }
}
