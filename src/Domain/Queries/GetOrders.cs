using Domain.DTOs;
using Shared.CQRS;

namespace Domain.Queries;

public class GetOrders : IQuery<GetOrdersResult>
{
    public string Filter { get; set; }
    public int Skip { get; set; }
    public int Top { get; set; } = 10;
}

public class GetOrdersResult
{
    public IEnumerable<OrderDto> Orders { get; set; }
}