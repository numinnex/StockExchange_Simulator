using Application.Common.Intefaces;
using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Application.Stocks.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Stocks.Queries;


public sealed record GetStockbyNameQuery : IRequest<Result<List<StockDto>>>
{
    public required string Name { get; init; } 
}

public sealed class GetStockByNameQueryHandler : IRequestHandler<GetStockbyNameQuery, Result<List<StockDto>>>
{
    private readonly IMapper _mapper;
    private readonly IStockRepository _stockRepository;
    private readonly IStockClient _stockClient;

    public GetStockByNameQueryHandler( IMapper mapper , IStockRepository stockRepository , IStockClient stockClient) 
    {
        _mapper = mapper;
        _stockRepository = stockRepository;
        _stockClient = stockClient;
    }
    
    public async Task<Result<List<StockDto>>> Handle(GetStockbyNameQuery request, CancellationToken cancellationToken)
    {
        var stocks = (await _stockRepository.
            GetAllAsync(cancellationToken,x => x.Name == request.Name));

        if (!stocks.Any())
            stocks = await _stockClient.GetStocksBySymbolAsync(request.Name);

        if (!stocks.Any())
        {
            return Result<List<StockDto>>.Failure(new List<StockDto>(), new Error[]
            {
                new Error() { Code = "empty_col" , Message = "The collection was empty"}
            });
        }
        
        await _stockRepository.AddRangeAsync(stocks, cancellationToken);
        
        var stocksMapped = _mapper.Map<List<StockDto>>(stocks);
        return Result<List<StockDto>>.Success(stocksMapped);
    }
}