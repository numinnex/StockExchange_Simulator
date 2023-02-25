using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;

namespace Application.Stocks.Queries;


public sealed record GetStockbyNameQuery : IRequest<Result<StockDto>>
{
    public required string Name { get; init; } 
}

public sealed class GetStockByNameQueryHandler : IRequestHandler<GetStockbyNameQuery, Result<StockDto>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetStockByNameQueryHandler(IApplicationDbContext ctx , IMapper mapper )
    {
        _ctx = ctx;
        _mapper = mapper;
    }
    
    public Task<Result<StockDto>> Handle(GetStockbyNameQuery request, CancellationToken cancellationToken)
    {
        
    }
}