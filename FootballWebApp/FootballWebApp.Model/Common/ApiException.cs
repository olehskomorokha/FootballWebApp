namespace FootballWebApp.Model.Common;

public class ApiException : Exception
{
    public string Code { get; }

    public ApiException(string code, string message = null, Exception inner = null)
        : base(message ?? code, inner)
    {
        Code = code;
    }
}