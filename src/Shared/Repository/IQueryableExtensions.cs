using AutoMapper;
using AutoMapper.QueryableExtensions;
using Community.OData.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData;
using Shared.CQRS;
using Shared.Exceptions;

namespace Shared.Repository;

public static class IQueryableExtensions
{
    public static async ValueTask<TPagedResult> GetPagedResult<TEntity, TDto, TPagedResult>(
        this IQueryable<TEntity> queryable, 
        PagedQuery query, 
        IMapper mapper,
        CancellationToken token = default) 
        where TPagedResult : PagedResult<TDto>, new()
        where TEntity : class
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                queryable = queryable.OData()
                    .Filter(query.Filter)
                    .ToOriginalQuery();
            }

            var totalCount = await queryable.CountAsync(token);

            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                queryable = queryable.OData()
                    .OrderBy(query.OrderBy)
                    .ToOriginalQuery();
            }

            var membersToExpand = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Expand))
            {
                membersToExpand = [.. query.Expand.Split(',', 
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
            }

            var projectedOrders = queryable
                .Skip(query.Skip ?? 0)
                .Take(query.Top ?? 100)
                .AsNoTracking()
                .ProjectTo<TDto>(
                    mapper.ConfigurationProvider, null, membersToExpand: [.. membersToExpand]);

            return new TPagedResult
            {
                TotalCount = totalCount,
                Items = await projectedOrders.ToListAsync(token)
            };
        }
        catch (Exception ex) when (ex is ArgumentException or ODataException)
        {
            throw new PagedQueryException(ex.Message, ex);
        }
    }
}
