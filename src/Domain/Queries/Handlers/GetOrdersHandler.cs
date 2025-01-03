using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.UnitOfWork;
using Shared.CQRS;
using Shared.Repository;

namespace Domain.Queries.Handlers;

public class GetOrdersHandler(
    IMyAppUnitOfWorkFactory _unitOfWorkFactory, 
    IMapper _mapper) : IQueryHandler<GetOrders, GetOrdersResult>
{
    public async ValueTask<GetOrdersResult> ExecuteAsync(GetOrders query, CancellationToken token = default)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var orders = unitOfWork.OrderRepository.GetAll();

        var result = await orders.GetPagedResult<Order, OrderDto, GetOrdersResult>(query, _mapper, token);

        return result;
    }
}
