using Shared.CQRS;
using AutoMapper;
using Domain.DTOs;
using Domain.UnitOfWork;

namespace Domain.Queries.Handlers;

public class GetOrderHandler(
    IMyAppUnitOfWorkFactory _unitOfWorkFactory, 
    IMapper _mapper) : IQueryHandler<GetOrder, GetOrderResult>
{
    public async ValueTask<GetOrderResult> ExecuteAsync(GetOrder query, CancellationToken cancellationToken = default)
    {
        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync();

        var order = await unitOfWork.OrderRepository.GetOrderWithEvents(query.Id, cancellationToken);
        
        return new GetOrderResult 
        { 
            Order = order != null ? _mapper.Map<OrderDto>(order) : null 
        };
    }
} 