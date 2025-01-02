﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Community.OData.Linq;
using Domain.DTOs;
using Domain.Exceptions;
using Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData;
using Shared.CQRS;

namespace Domain.Queries.Handlers;

public class GetOrdersHandler(
    IMyAppUnitOfWorkFactory unitOfWorkFactory, 
    IMapper mapper) : IQueryHandler<GetOrders, GetOrdersResult>
{
    public async ValueTask<GetOrdersResult> ExecuteAsync(GetOrders query, CancellationToken token = default)
    {
        using var unitOfWork = unitOfWorkFactory.Create();

        var orders = unitOfWork.OrderRepository.GetAll();

        try
        {
            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                orders = orders.OData()
                    .Filter(query.Filter)
                    .ToOriginalQuery();
            }

            var totalCount = await orders.CountAsync();

            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                orders = orders.OData()
                    .OrderBy(query.OrderBy)
                    .ToOriginalQuery();
            }

            var projectedOrders = orders
                .Skip(query.Skip ?? 0)
                .Take(query.Top ?? 100)
                .AsNoTracking()
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider);

            return new GetOrdersResult
            {
                TotalCount = totalCount,
                Items = await projectedOrders.ToListAsync(token)
            };
        }
        catch (ODataException ex)
        {
            throw new PagedQueryException(ex.Message, ex);
        }
    }
}
