using Application.Common.Interfaces;
using Application.Common.Models;
using Contracts.V1.Responses;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Orders.Commands;

public sealed record StopOrderQuantityCommand(string StockId, decimal Quantity, decimal StopPrice,string UserId, bool IsBuy) 
    :IRequest<Result<StopOrderResponse>>;
public sealed class StopOrderQuantityCommandHandler: IRequestHandler<StopOrderQuantityCommand , Result<StopOrderResponse>>
{
    private readonly IMatchingEngine _matchingEngine;
    private readonly IStockUtils _stockUtils;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StopOrderQuantityCommandHandler(IMatchingEngine matchingEngine, IStockUtils stockUtils,
        IDateTimeProvider dateTimeProvider )
    {
        _matchingEngine = matchingEngine;
        _stockUtils = stockUtils;
        _dateTimeProvider = dateTimeProvider;
    }
    public async Task<Result<StopOrderResponse>> Handle(StopOrderQuantityCommand request, CancellationToken cancellationToken)
    {
        var stockSymbol = await _stockUtils.GetStockSymbolByStockId(Guid.Parse(request.StockId));
        
        var order = new StopOrder()
        {
            Id = new Guid(),
            IsTriggered = false,
            IsBuy = request.IsBuy,
            StockId = Guid.Parse(request.StockId),
            UserId = request.UserId,
            Timestamp = _dateTimeProvider.Now,
            OpenQuantity = request.Quantity,
            StopPrice = request.StopPrice,
            //FeeId is hardcoded, don't have good idea where it could potentially live
            //maybe based on market?
            FeeId = 1,
            Type = request.IsBuy ? TradeType.StopBuy : TradeType.StopSell,
            Symbol = stockSymbol
        };
        await _matchingEngine.AddOrder(order , cancellationToken);

        return Result<StopOrderResponse>.Success(new StopOrderResponse
        {
            StockId = request.StockId,
            Price = request.StopPrice * request.Quantity
        });
    }}