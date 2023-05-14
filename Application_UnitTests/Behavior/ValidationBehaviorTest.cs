using Application.Common.Interfaces;
using Application.Orders.Commands;
using Application.Stocks.Queries;
using FakeItEasy;
using FluentValidation.TestHelper;
using Xunit;

namespace Application_Tests.Behavior;

public sealed class ValidationBehaviorTest
{
    
    private Guid _guid; 
    
    private GetStockQueryValidator _validator;
    private MarketOrderAmountCommandValidation _orderAmountValidator;
    private MarketOrderQuantityCommandValidation _orderQuantityValidator;
    private IStockUtils _stockUtils;
    private IIdentityService _identityService;
    public ValidationBehaviorTest()
    {
        _guid = new Guid();
        _validator = new GetStockQueryValidator();
        _stockUtils = A.Fake<IStockUtils>();
        _identityService = A.Fake<IIdentityService>();
        _orderAmountValidator = new MarketOrderAmountCommandValidation(_stockUtils , _identityService);
        _orderQuantityValidator = new MarketOrderQuantityCommandValidation(_stockUtils , _identityService);
        
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

    [Fact]
    public async Task ShouldHaveErrorsWhenMarketOrderQuantityIsLowerThanZero()
    {
        var query = new MarketOrderQuantityCommand("test" , -2 , "test" , true);
        var result = await _orderQuantityValidator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.Quantity);
    }
    //Create test for MarketOrderAmountCommand when guid is not valid
    [Fact]
    public async Task ShouldHaveErrorsWhenMarketOrderQuantityGuidIsNotValid()
    {
        var query = new MarketOrderQuantityCommand("test" , 12 , "test" , true);
        var result = await _orderQuantityValidator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.StockId);
        result.ShouldHaveValidationErrorFor(query => query.UserId);
    }
    [Fact]
    public async Task ShouldHaveErrorsWhenMarketOrderAmountIsLowerThanZero()
    {
        var query = new MarketOrderAmountCommand("test" , -2 , "test" , true);
        var result = await _orderAmountValidator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.Amount);
    }
    //Create test for MarketOrderAmountCommand when guid is not valid
    [Fact]
    public async Task ShouldHaveErrorsWhenMarketOrderAmountGuidIsNotValid()
    {
        var query = new MarketOrderAmountCommand("test" , 12 , "test" , true);
        var result = await _orderAmountValidator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.StockId);
        result.ShouldHaveValidationErrorFor(query => query.UserId);
    }
    [Fact]
    public async Task ShouldHaveErrorsWhenMarketOrderAmountIsSell()
    {
        var query = new MarketOrderAmountCommand(_guid.ToString() , 12 , _guid.ToString() , false);
        var result = await _orderAmountValidator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(query => query.IsBuy);
    }

}