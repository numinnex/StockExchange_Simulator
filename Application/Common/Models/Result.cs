namespace Application.Common.Models;

public sealed class Result<TValue>
{
    public required bool IsSuccess { get; init; }
    public required TValue Value { get; init; }
    public required IEnumerable<Error> Errors { get; init; }
    public static Result<TValue> Success(TValue value) => new Result<TValue>
    {
        IsSuccess = true, Errors = Error.None, Value = value
    };
    public static Result<TValue> Failure(TValue value, IEnumerable<Error> error) => new Result<TValue>
    {
        IsSuccess = true, Errors = error, Value = value
    };
}