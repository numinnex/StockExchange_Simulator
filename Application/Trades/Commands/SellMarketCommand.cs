using Application.Common.Interfaces;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Trades.Commands;

public sealed record SellMarketCommand(string StockId, int Quantity, string UserId) : IRequest<Result<SellResponse>>;

public sealed class SellMarketCommandHandler : IRequestHandler<SellMarketCommand, Result<SellResponse>>
{
    private readonly ITradeService _tradeService;

    public SellMarketCommandHandler(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }
    
    public async Task<Result<SellResponse>> Handle(SellMarketCommand request, CancellationToken cancellationToken)
    {
        var response = await _tradeService.SellMarketAsync(new Guid(request.StockId), request.Quantity, request.UserId,
            cancellationToken);
        return response;
    }
}