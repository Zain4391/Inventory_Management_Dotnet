using System.Net;

namespace Inventory_Management_dotnet.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, HttpStatusCode.Unauthorized) {}
}