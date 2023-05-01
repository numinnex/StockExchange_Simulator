using Application.Common.Interfaces;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Trades.Commands;

public record MarketOrderCommand(string StockId, decimal Quantity, string UserId, bool IsBuy) : IRequest<Result<MarketOrderResponse>>
{
}
public sealed class BuyMarketCommandHandler : IRequestHandler<MarketOrderCommand, Result<MarketOrderResponse>>
{

    public BuyMarketCommandHandler()
    {

    }
    public async Task<Result<MarketOrderResponse>> Handle(MarketOrderCommand request, CancellationToken cancellationToken)
    {
        // var response = await _tradeService.BuyMarketAsync(new Guid(request.StockId), request.Quantity, request.UserId,
        //     cancellationToken);
        // return response;
        throw new NotImplementedException();
    }
}