using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Portfolios;

public sealed record GetUserSecuritiesQuery(string userId): IRequest<Result<List<UserSecuritiesResponse>>>;

public sealed class
    GetUserSecurityQueryHandler : IRequestHandler<GetUserSecuritiesQuery, Result<List<UserSecuritiesResponse>>>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStockUtils _stockUtils;
    private readonly IStockClient _stockClient;

    public GetUserSecurityQueryHandler(IPortfolioRepository portfolioRepository,
        IStockUtils stockUtils,
        IStockClient stockClient)
    {
        _portfolioRepository = portfolioRepository;
        _stockUtils = stockUtils;
        _stockClient = stockClient;
    }
    public async Task<Result<List<UserSecuritiesResponse>>> Handle(GetUserSecuritiesQuery request, CancellationToken cancellationToken)
    {
        List<UserSecuritiesResponse> result = new();
        var securities = await _portfolioRepository.GetSecuritiesByUserIdAsync(request.userId, cancellationToken);
        foreach (var security in securities)
        {
            try
            {
                var symbol = await _stockUtils.GetStockSymbolByStockId(security.StockId);
                var price = await _stockClient.GetRealtimePrice(symbol);
                result.Add(new UserSecuritiesResponse
                {
                    Quantity = Math.Round(security.Quantity,2),
                    PurchasedPrice = Math.Round(security.PurchasedPrice,2),
                    CurrentPrice = Math.Round(price, 2),
                    TotalValue = Math.Round(security.Quantity * price, 2),
                    Symbol = symbol
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return Result<List<UserSecuritiesResponse>>.Failure(new[]
                    {
                        new Error
                        {
                            Message = "Something went wrong",
                            Code = "Error"
                        }
                    }
                );
            }
        }
        return Result<List<UserSecuritiesResponse>>.Success(result);
    }
}