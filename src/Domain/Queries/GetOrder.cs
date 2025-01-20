using Domain.DTOs;
using Shared.CQRS;

namespace Domain.Queries;

public class GetOrder : IQuery<GetOrderResult>
{
    public Guid Id { get; init; }
}

public class GetOrderResult
{
    public OrderDto Order { get; init; }
} 