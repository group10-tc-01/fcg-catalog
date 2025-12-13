using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FCG.Catalog.Domain.Exception
{
    [ExcludeFromCodeCoverage]
    public class ConflictException : BaseException
    {
        public ConflictException(string message) : base(HttpStatusCode.Conflict, message) { }
        public ConflictException(string message, System.Exception innerException) : base(HttpStatusCode.Conflict, message, innerException) { }

    }
}
