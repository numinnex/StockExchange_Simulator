using FluentValidation;

namespace Application.Trades.Queries;

public sealed class GetActiveMarketOrdersQueryValidation : AbstractValidator<GetActiveMarketOrdersQuery>
{
    public GetActiveMarketOrdersQueryValidation()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0)
            .WithMessage("Page number must be greater than 0");
        RuleFor(x => x.PageSize).GreaterThan(0)
            .WithMessage("Page size must be greater than 0");
    }
}