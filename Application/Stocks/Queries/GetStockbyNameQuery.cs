using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Application.Common.Models.ReadModels;
using AutoMapper;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Stocks.Queries;


public sealed record GetStockbyNameQuery(string Symbol) : IRequest<Result<List<StockResponse>>>;
public sealed class GetStockByNameQueryHandler : IRequestHandler<GetStockbyNameQuery, Result<List<StockResponse>>>
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
    
    public async Task<Result<List<StockResponse>>> Handle(GetStockbyNameQuery request, CancellationToken cancellationToken)
    {
        var stocks = (await _stockRepository.
            GetAllAsync(cancellationToken,x => x.Symbol == request.Symbol , "TimeSeries,Trades"));

        if (stocks.Any())
        {
            var stocksMapped = _mapper.Map<List<StockResponse>>(stocks);
            return Result<List<StockResponse>>.Success(stocksMapped);
        }

        stocks = await _stockClient.GetStocksBySymbolAsync(request.Symbol);
        if (!stocks.Any())
        {
            return Result<List<StockResponse>>.Failure( new []
            {
                new Error() { Code = "Not Found" , Message = "The collection was empty"}
            });
        }
        
        await _stockRepository.AddRangeAsync(stocks, cancellationToken);
        await _stockRepository.SaveChangesAsync(cancellationToken);
        
        var stocksFromApiMapped = _mapper.Map<List<StockResponse>>(stocks);
        return Result<List<StockResponse>>.Success(stocksFromApiMapped);
    }
}