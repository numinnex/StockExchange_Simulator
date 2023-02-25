using Application.Stocks.Queries;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mappings;

public sealed class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Stock, StockDto>().ReverseMap();
    } 
}