using Domain.DTOs;
using Shared.CQRS;

namespace Domain.Queries;

public class GetOrder : ICacheableQuery<GetOrderResult>
{
    public Guid Id { get; init; }
    public string CacheKey => $"order-{Id}";
}

public class GetOrderResult
{
    public OrderDto Order { get; init; }
} 