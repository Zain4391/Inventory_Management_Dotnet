using System.Net;

namespace Inventory_Management_dotnet.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string message) : base(message, HttpStatusCode.Conflict) {}
}