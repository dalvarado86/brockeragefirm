using AutoMapper;
using Domain.Entities;

namespace Application.Orders
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, IssuerDto>()
                .ForMember(d => d.IssuerName, o => o.MapFrom(s => s.IssuerName))
                .ForMember(d => d.TotalShares, o => o.MapFrom(s => s.TotalShares))
                .ForMember(d => d.SharePrice, o => o.MapFrom(s => s.SharePrice));
        }
    }
}
