using System.Net;

namespace Inventory_Management_dotnet.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string resourseName, object key)
        : base($"{resourseName} with ID: {key} not found", HttpStatusCode.NotFound) {}
}