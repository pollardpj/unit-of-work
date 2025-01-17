using Domain.DTOs;
using Shared.CQRS;

namespace Domain.Queries;

public class GetOrder : IQuery<GetOrderResult>
{
    public Guid Id { get; set; }
}

public class GetOrderResult
{
    public OrderDto Order { get; set; }
} 