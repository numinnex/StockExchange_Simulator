using Application.Common.Interfaces.Repository;
using Application.Common.Models;
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    public GetStockByNameQueryHandler( IMapper mapper , IStockRepository stockRepository, IHttpClientFactory httpClientFactory)
    {
        _mapper = mapper;
        _stockRepository = stockRepository;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("twelveData");
    }
    
    public async Task<Result<List<StockDto>>> Handle(GetStockbyNameQuery request, CancellationToken cancellationToken)
    {
        var stocks = (await _stockRepository.
            GetAllAsync(x => x.Name == request.Name)).ToList();
        var stocksMapped = _mapper.Map<List<StockDto>>(stocks);

        return stocksMapped.Count > 0
            ? Result<List<StockDto>>.Success(stocksMapped)
            : Result<List<StockDto>>.Failure(stocksMapped, new Error[]
            {
                new Error() { Code = "empty_col" , Message = "The collection was empty"}
            });
    }
}