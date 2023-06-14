using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
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
    private readonly IPortfolioRepository _portfolioRepository;

    public StopOrderQuantityCommandHandler(IMatchingEngine matchingEngine, IStockUtils stockUtils,
        IDateTimeProvider dateTimeProvider , IPortfolioRepository portfolioRepository)
    {
        _matchingEngine = matchingEngine;
        _stockUtils = stockUtils;
        _dateTimeProvider = dateTimeProvider;
        _portfolioRepository = portfolioRepository;
    }
    public async Task<Result<StopOrderResponse>> Handle(StopOrderQuantityCommand request, CancellationToken cancellationToken)
    {
        var stockSymbol = await _stockUtils.GetStockSymbolByStockId(Guid.Parse(request.StockId));
        
        var portfolio = await _portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (portfolio is null)
        {
            return Result<StopOrderResponse>.Failure(new[]
            {
                new Error
                {
                    Code = "PortfolioDoesntExist",
                    Message = "Portfolio doesn't exist"
                }
            });
        }
        if (!request.IsBuy)
        {
            var securities = portfolio.Securities
                .Where(x => x.StockId == Guid.Parse(request.StockId)).ToList();
            bool hasQuantity = false;
            foreach (var security in securities)
            {
                if (security.Quantity >= request.Quantity)
                {
                    hasQuantity = true;
                }
            }
            if (!hasQuantity || portfolio.Securities.Count() == 0)
            {
                return Result<StopOrderResponse>.Failure(new[]
                {
                    new Error
                    {
                        Code = "NotEnoughQuantity", 
                        Message = "You don't have enough quantity of this stock"
                    }
                });
            }
        }
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