using FluentValidation;

namespace Application.Stocks.Queries;

public sealed class GetStockQueryValidator : AbstractValidator<GetStockbyNameQuery>
{
    public GetStockQueryValidator()
    {
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Symbol).NotNull();
        RuleFor(x => x.Symbol).MinimumLength(1);
    } 
}