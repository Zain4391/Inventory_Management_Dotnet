using System.Net;

namespace Inventory_Management_dotnet.Exceptions;

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest) {}
}