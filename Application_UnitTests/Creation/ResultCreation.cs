using Application.Common.Models;
using FluentAssertions;
using Xunit;

namespace Application_Tests.Creation;

public sealed class ResultCreation
{
    [Fact]
    public void ShouldCreateEmptyErrorsArrayWhenSuccessful()
    {
        var result = Result<Test>.Success(new Test("test", "test"));
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldHaveArrayWithErrorsWhenFailed()
    {
        var result = Result<Test>.Failure(new[] { new Error() { Code = "xd", Message = "xd" } });
        result.Errors.Should().NotBeEmpty();
    }
}
public record Test(string name, string xd);