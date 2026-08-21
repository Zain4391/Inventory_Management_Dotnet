using System.Net;

namespace Inventory_Management_dotnet.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode statusCode {get;}

    public AppException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError) : base(message)
    {
        statusCode = httpStatusCode;
    }
}