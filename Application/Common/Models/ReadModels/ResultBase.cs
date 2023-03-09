namespace Application.Common.Models.ReadModels;

public sealed class ResultBase
{
    
    public required bool IsSuccess { get; init; }
    public required IEnumerable<Error> Errors { get; init; }
}