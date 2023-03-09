namespace Application.Common.Models;

public interface IResult
{
    public bool IsSuccess { get; }
    public IEnumerable<Error> Errors { get; }
}