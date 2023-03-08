using Application.Stocks.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mappings;

public sealed class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Stock, StockDto>().ReverseMap();
        CreateMap<StockSnapshot, StockSnapShotDto>().ReverseMap();
        CreateMap<TimeSeries, TimeSeriesDto>().ReverseMap();
    } 
}