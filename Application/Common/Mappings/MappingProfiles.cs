using AutoMapper;
using Contracts.V1.Responses;
using Domain.Entities;
using TimeSeriesResponse = Contracts.V1.Responses.TimeSeriesResponse;

namespace Application.Common.Mappings;

public sealed class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<MarketOrder, MarketTradeResponse>().ReverseMap();
        CreateMap<Stock, StockResponse>()
            .ForPath(x => x.Price,
                opt => opt.MapFrom(x => x.Price.Value))
            .ForPath(x => x.Change,
                opt => opt.MapFrom(x => x.Change!.Value))
            .ReverseMap();
        CreateMap<StockSnapshot, StockSnapshotResponse>().ReverseMap();
        CreateMap<TimeSeries, TimeSeriesResponse>().ReverseMap();
        CreateMap<MarketOrder, MarketOrderResponse>()
            .ForPath(x => x.Price,
                opt => opt.MapFrom(x => x.Price.Value))
            .ForPath(x => x.OpenQuantity,
                opt => opt.MapFrom(x => x.OpenQuantity.Value))
            .ForPath(x => x.OrderAmount,
                opt => opt.MapFrom(x => x.OrderAmount.Value))
            .ReverseMap();

    }
}