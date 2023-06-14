using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Portfolios;

public record GetUserPortfolioQuery(string UserId) : IRequest<Result<PortfolioResponse>>;
public sealed class GetUserPortfolioQueryHandler: IRequestHandler<GetUserPortfolioQuery, Result<PortfolioResponse>>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStockUtils _stockUtils;
    private readonly IStockClient _stockClient;

    public GetUserPortfolioQueryHandler(IPortfolioRepository portfolioRepository, IStockUtils stockUtils, IStockClient stockClient)
    {
        _portfolioRepository = portfolioRepository;
        _stockUtils = stockUtils;
        _stockClient = stockClient;
    }
    public async Task<Result<PortfolioResponse>> Handle(GetUserPortfolioQuery request, CancellationToken cancellationToken)
    {
        var result = await _portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
         
        if (result is not null)
        {
            List<SecurityResponse> securities = new();

            foreach (var sec in result.Securities)
            {
                var stockSymbol = await _stockUtils.GetStockSymbolByStockId((Guid)sec.StockId);
                var currentPrice = await _stockClient.GetRealtimePrice(stockSymbol);
                securities.Add(
                    new SecurityResponse
                    {
                        Timestamp = sec.Timestamp,
                        Quantity = sec.Quantity,
                        Symbol = stockSymbol,
                        PurchasedPrice = sec.PurchasedPrice,
                        CurrentPrice = currentPrice,
                        TotalValue = currentPrice * sec.Quantity,
                    }
                );
            }
            return Result<PortfolioResponse>.Success(new PortfolioResponse
            {
                ValueSnapshots = result.ValueSnapshots.Select(x => new ValueSnapshotResponse
                {
                    Timestamp = x.Timestamp,
                    Value = x.Value
                }).ToList(),
                TotalValue = result.TotalValue,
                Securities = securities
            });
        }
        return Result<PortfolioResponse>.Failure(new []
        {
            new Error
            {
                Message = "Portfolio not found",
                Code = "Not Found"
            }
        });
}
}