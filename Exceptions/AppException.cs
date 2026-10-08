
namespace OrderAndPaymentAPI.Exceptions;
public class AppException : Exception
{
    public int statusCode {get;}
    public AppException(string message, int StatusCode = 400) : base(message)
    {
        statusCode = StatusCode;
    }
}