using Application.Common.Interfaces;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Trades.Commands;

public sealed record BuyMarketCommand(string StockId, int Quantity, string UserId) : IRequest<Result<BuyResponse>>;

public sealed class BuyMarketCommandHandler : IRequestHandler<BuyMarketCommand, Result<BuyResponse>>
{
    private readonly ITradeService _tradeService;

    public BuyMarketCommandHandler(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }
    public async Task<Result<BuyResponse>> Handle(BuyMarketCommand request, CancellationToken cancellationToken)
    {
        var response = await _tradeService.BuyMarketAsync(new Guid(request.StockId), request.Quantity, request.UserId,
            cancellationToken);
        return response;
    }
}