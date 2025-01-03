using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.Entities;
using Domain.Queries;
using MyAppAPI.Models;

namespace MyAppAPI.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<CreateOrderRequest, CreateOrder>();

            CreateMap<OrderEvent, OrderEventDto>();

            CreateMap<Order, OrderDto>()
                .ForMember(o => o.Events, o =>
                {
                    o.MapFrom(s => s.Events);
                    o.ExplicitExpansion();
                });

            CreateMap<GetOrdersRequest, GetOrders>();
        }
    }
}
