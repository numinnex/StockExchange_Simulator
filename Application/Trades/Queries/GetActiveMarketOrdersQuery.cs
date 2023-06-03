using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using AutoMapper;
using Contracts.V1.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Application.Trades.Queries;

public sealed record GetActiveMarketOrdersQuery(int PageSize, int PageNumber) 
    : IRequest<Result<List<MarketOrderResponse>>>;

public sealed class GetActiveMarketOrdersQueryHandler 
    : IRequestHandler<GetActiveMarketOrdersQuery , Result<List<MarketOrderResponse>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetActiveMarketOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }
    public async Task<Result<List<MarketOrderResponse>>> Handle(GetActiveMarketOrdersQuery request, CancellationToken cancellationToken)
    {
        var result =
            await _orderRepository.GetPaginatedActiveMarketOrdersAsync(request.PageNumber, request.PageSize,
                cancellationToken);
        if (!result.Any())
        {
            return Result<List<MarketOrderResponse>>.Failure(new[]
            {
                new Error() { Code = "Not Found" , Message = "The collection was empty"}
            });
        }
        var mapperResult = _mapper.Map<List<MarketOrderResponse>>(result);
        return Result<List<MarketOrderResponse>>.Success(mapperResult);
    }
}