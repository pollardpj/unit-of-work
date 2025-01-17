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
            // Apply OData filtering if specified
            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                queryable = queryable.OData()
                    .Filter(query.Filter)
                    .ToOriginalQuery();
            }

            // Get total count before pagination
            var totalCount = await queryable.CountAsync(token);

            // Apply OData ordering if specified
            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                queryable = queryable.OData()
                    .OrderBy(query.OrderBy)
                    .ToOriginalQuery();
            }

            // Parse expand parameters
            var membersToExpand = !string.IsNullOrWhiteSpace(query.Expand)
                ? query.Expand.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                : [];

            // Apply pagination, projection and execute query
            var items = await queryable
                .Skip(query.Skip ?? 0)
                .Take(Math.Min(query.Top ?? 100, 1000)) // Add max limit of 1000 items
                .AsNoTracking()
                .ProjectTo<TDto>(
                    mapper.ConfigurationProvider, null, membersToExpand: [.. membersToExpand])
                .ToListAsync(token);

            return new TPagedResult
            {
                TotalCount = totalCount,
                Items = items
            };
        }
        catch (Exception ex) when (ex is ArgumentException or ODataException)
        {
            throw new BadRequestException($"Invalid query parameters: {ex.Message}", ex);
        }
    }
}
