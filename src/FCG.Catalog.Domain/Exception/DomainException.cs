using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FCG.Catalog.Domain.Exception
{
    [ExcludeFromCodeCoverage]
    public class DomainException : BaseException
    {
        public DomainException(string message) : base(HttpStatusCode.BadRequest, message) { }
        public DomainException(string message, System.Exception innerException) : base(HttpStatusCode.BadRequest, message, innerException) { }
    }
}
