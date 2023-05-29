using Application.Auth.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Stocks.Queries;


public sealed record SymbolLookupQuery(string Symbol): IRequest<Result<List<SymbolLookupResponse>>>;

public sealed class SymbolLookupQueryHandler: IRequestHandler<SymbolLookupQuery, Result<List<SymbolLookupResponse>>>
{
    private readonly IStockClient _stockClient;

    public SymbolLookupQueryHandler(IStockClient stockClient)
    {
        _stockClient = stockClient;
    }
    public async Task<Result<List<SymbolLookupResponse>>> Handle(SymbolLookupQuery request, CancellationToken cancellationToken)
    {
        var result = await _stockClient.SymbolLookupAsync(request.Symbol);

        if (result.Count == 0)
        {
            return Result<List<SymbolLookupResponse>>.Failure( new []
            {
                new Error(){Code = "Wrong Symbol", Message = $"{request.Symbol} is not valid symbol"}
            });
        }
        return Result<List<SymbolLookupResponse>>.Success(result); 
    }
}