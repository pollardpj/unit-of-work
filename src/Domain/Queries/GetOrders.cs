using Domain.DTOs;
using Shared.CQRS;

namespace Domain.Queries;

public class GetOrders : IQuery<GetOrdersResult>
{
}

public class GetOrdersResult
{
    public IEnumerable<OrderDto> Orders { get; set; }
}