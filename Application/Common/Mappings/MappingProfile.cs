using Application.Accounts.Models;
using Application.Orders.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, IssuerDto>()
                .ForMember(d => d.IssuerName, o => o.MapFrom(s => s.IssuerName))
                .ForMember(d => d.TotalShares, o => o.MapFrom(s => s.TotalShares))
                .ForMember(d => d.SharePrice, o => o.MapFrom(s => s.SharePrice));

            CreateMap<Order, OrderDto>()
                .ForMember(d => d.TimeStamp, o => o.MapFrom(s => s.TimeStamp))
                .ForMember(d => d.Operation, o => o.MapFrom(s => s.Operation))
                .ForMember(d => d.IssuerName, o => o.MapFrom(s => s.IssuerName))
                .ForMember(d => d.TotalShares, o => o.MapFrom(s => s.TotalShares))
                .ForMember(d => d.SharePrice, o => o.MapFrom(s => s.SharePrice));

            CreateMap<Stock, StockDto>()
                .ForMember(d => d.IssuerName, o => o.MapFrom(s => s.IssuerName))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity));
        }
    }
}
