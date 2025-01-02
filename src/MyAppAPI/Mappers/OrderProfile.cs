using AutoMapper;
using Domain.DTOs;
using Domain.Entities;

namespace MyAppAPI.Mappers
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderEvent, OrderEventDto>();
            CreateMap<Order, OrderDto>();
        }
    }
}
