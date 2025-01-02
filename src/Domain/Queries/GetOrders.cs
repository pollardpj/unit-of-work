using Domain.DTOs;
using Shared.CQRS;

namespace Domain.Queries;

public class GetOrders : PagedQuery, IQuery<GetOrdersResult>
{
}

public class GetOrdersResult : PagedResult<OrderDto>
{
}