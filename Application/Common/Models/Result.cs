using Application.Common.Interfaces;

namespace Application.Common.Models;

public sealed class Result<TValue> : IResult
{
    public required bool IsSuccess { get; init; }
    public TValue Value { get; init; }

    public required IEnumerable<Error> Errors { get; init; }
    public static Result<TValue> Success(TValue value) => new Result<TValue>
    {
        IsSuccess = true, Errors = Error.None, Value = value
    };
    public static Result<TValue> Failure( IEnumerable<Error> error) => new Result<TValue>
    {
        IsSuccess = false, Errors = error 
    };
}
