using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Contracts.V1.Responses;
using Domain.Enums;
using MediatR;

namespace Application.Trades.Queries;

public sealed record GetUserTradesQuery(string userId) : IRequest<Result<List<UserTradeResponse>>>;

public sealed class GetUserTradesQueryHandler
    : IRequestHandler<GetUserTradesQuery, Result<List<UserTradeResponse>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetUserTradesQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<UserTradeResponse>>> Handle(GetUserTradesQuery request,
        CancellationToken cancellationToken)
    {
        var marketOrders = await _orderRepository
            .GetAllMarketOrdersAsync(cancellationToken, x => x.UserId == request.userId);
        var stopOrders =
            await _orderRepository.GetAllStopOrdersAsync(cancellationToken, x => x.UserId == request.userId);
        
        var userMarketOrders = marketOrders.Select(x =>
        {
            return new UserTradeResponse
            {
                IsBuy = x.IsBuy,
                Price = x.Price,
                Quantity = x.OpenQuantity ?? 0,
                OrderAmount = x.OrderAmount ?? 0,
                Symbol = x.Symbol,
                Timestamp = x.Timestamp,
                Type = x.Type switch
                {
                    TradeType.BuyMarket => "Market",
                    TradeType.SellMarket => "Market",
                    TradeType.StopBuy => "Stop",
                    TradeType.StopSell => "Stop",
                    _ => "TROLOLOLO"
                }
            };
        }).ToList();
        var userStopOrders = stopOrders.Select (x =>
        {
            return new UserTradeResponse
            {
                IsBuy = x.IsBuy,
                Price = x.StopPrice,
                Quantity = x.OpenQuantity ?? 0,
                OrderAmount = 0,
                Symbol = x.Symbol,
                Timestamp = x.Timestamp,
                Type = x.Type switch
                {
                    TradeType.BuyMarket => "Market",
                    TradeType.SellMarket => "Market",
                    TradeType.StopBuy => "Stop",
                    TradeType.StopSell => "Stop",
                    _ => "TROLOLO"
                }
            };
        }).ToList();

        return Result<List<UserTradeResponse>>.Success(
            userMarketOrders.Concat(userStopOrders).OrderBy(x => x.Timestamp).ToList()
        );
    }
}