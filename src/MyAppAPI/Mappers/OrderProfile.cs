using AutoMapper;
using Domain.Commands;
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
            CreateMap<OrderRequest, CreateOrder>();

            CreateMap<OrderEvent, OrderEventDto>();
            CreateMap<Order, OrderDto>();

            CreateMap<GetOrdersRequest, GetOrders>();
        }
    }
}
