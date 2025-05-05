namespace Microblogging.Application.Common;

public class Result
{
    public bool Success { get; }
    public string? Error { get; }

    public bool Failure => !Success;

    protected Result(bool success, string? error)
    {
        Success = success;
        Error = error;
    }

    public static Result SuccessResult() => new(true, null);

    public static Result FailureResult(string error) =>
        string.IsNullOrWhiteSpace(error)
            ? throw new ArgumentNullException(nameof(error))
            : new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, string? error)
        : base(success, error) => Value = value;

    public static Result<T> SuccessResult(T value) =>
        value is null
            ? throw new ArgumentNullException(nameof(value))
            : new(true, value, null);

    public static new Result<T> FailureResult(string error) =>
        string.IsNullOrWhiteSpace(error)
            ? throw new ArgumentNullException(nameof(error))
            : new(false, default, error);
}
