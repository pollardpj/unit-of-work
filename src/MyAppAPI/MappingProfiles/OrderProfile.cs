using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.Entities;
using Domain.Queries;
using MyAppAPI.Models;

namespace MyAppAPI.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<CreateOrderRequest, CreateOrder>();
        CreateMap<UpdateOrderRequest, UpdateOrder>()
            .ForMember(d => d.RowVersion, o => o.MapFrom(s => Convert.FromBase64String(s.RowVersion)));

        CreateMap<OrderEvent, OrderEventDto>();

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Events, o =>
            {
                o.MapFrom(s => s.Events);
                o.ExplicitExpansion();
            });

        CreateMap<GetOrdersRequest, GetOrders>();
    }
}
