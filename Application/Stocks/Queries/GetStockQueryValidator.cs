using FluentValidation;

namespace Application.Stocks.Queries;

public sealed class GetStockQueryValidator : AbstractValidator<GetStockbyNameQuery>
{
    public GetStockQueryValidator()
    {
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(10);
    } 
}