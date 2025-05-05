namespace Microblogging.Api.Responses;

public class ErrorMsg
{
    public string[] Errors { get; private set; }
    public ErrorMsg(string[] errors)
    {
        Errors = errors;
    }
}
public static class ErrorResults
{
    public static IResult MissingHeader(string headerName)
        => Results.BadRequest(new ErrorMsg(new[] { $"Falta el header {headerName}" }));

    public static IResult InvalidHeader(string headerName)
        => Results.BadRequest(new ErrorMsg(new[] { $"{headerName} no es válido" }));

    public static IResult Custom(params string[] messages)
        => Results.BadRequest(new ErrorMsg(messages));
}
