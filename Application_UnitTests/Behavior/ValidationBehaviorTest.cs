using Application.Stocks.Queries;
using FluentValidation.TestHelper;
using Xunit;

namespace Application_Tests.Behavior;

public sealed class ValidationBehaviorTest
{
    private GetStockQueryValidator _validator;
    public ValidationBehaviorTest()
    {
        _validator = new GetStockQueryValidator();
    }

    [Fact]
    public async Task ShouldHaveErrorsWhenSymbolIsEmpty()
    {
        var query = new GetStockbyNameQuery("");
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.Symbol);
    }
    [Fact]
    public async Task ShouldHaveErrorsWhenNull()
    {
        var query = new GetStockbyNameQuery(null);
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.Symbol);
    }
    [Fact]
    public async Task ShouldHaveErrorsWhenLengthGreaterThan10()
    {
        var query = new GetStockbyNameQuery("jashdawhkjdgahdvadhgjasdw");
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.Symbol);
    }
    
}