namespace GameBackendSDK.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string ErrorMessage { get; }

    public ApiException(int statusCode, string errorMessage)
        : base($"API Error {statusCode}: {errorMessage}")
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}