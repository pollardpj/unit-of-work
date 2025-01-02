using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.DTOs;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.CQRS;

namespace Domain.Queries.Handlers;

public class GetOrdersHandler(IMyAppUnitOfWorkFactory unitOfWorkFactory, IMapper mapper) : IQueryHandler<GetOrders, GetOrdersResult>
{
    public async ValueTask<GetOrdersResult> ExecuteAsync(GetOrders query)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        var orders = unitOfWork.OrderRepository.GetAll()
            .Take(20)
            .ProjectTo<OrderDto>(mapper.ConfigurationProvider);

        return new GetOrdersResult
        {
            Orders = await orders.ToListAsync()
        };
    }
}
