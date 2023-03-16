using System.Windows.Input;
using Application.Common.Interfaces;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Trades.Commands;

public sealed record BuyMarketCommand(string stockId, int quantity, string userId) : IRequest<Result<BuyResponse>>;

public sealed class BuyMarketCommandHandler : IRequestHandler<BuyMarketCommand, Result<BuyResponse>>
{
    private readonly ITradeService _tradeService;

    public BuyMarketCommandHandler(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }
    public async Task<Result<BuyResponse>> Handle(BuyMarketCommand request, CancellationToken cancellationToken)
    {
        var response = await _tradeService.BuyMarketAsync(new Guid(request.stockId), request.quantity, request.userId, cancellationToken);
        return response;
    }
}