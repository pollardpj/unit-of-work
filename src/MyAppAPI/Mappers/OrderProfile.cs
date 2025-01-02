using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Queries;
using MyAppAPI.Models;

namespace MyAppAPI.Mappers
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderEvent, OrderEventDto>();
            CreateMap<Order, OrderDto>();

            CreateMap<GetOrdersRequest, GetOrders>();
        }
    }
}
