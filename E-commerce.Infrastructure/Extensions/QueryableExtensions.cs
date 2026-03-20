using System.Linq.Expressions;
using E_commerce.Core.Common;

namespace E_commerce.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortColumn, string? sortDirection, Dictionary<string, Expression<Func<T, object>>> columnsMap)
    {
        if (string.IsNullOrEmpty(sortColumn) || !columnsMap.ContainsKey(sortColumn.ToLower()))
        {
            return query;
        }

        var selectedColumn = columnsMap[sortColumn.ToLower()];
        var isDescending = sortDirection?.ToLower() == "desc";

        return isDescending 
            ? query.OrderByDescending(selectedColumn) 
            : query.OrderBy(selectedColumn);
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? searchValue, Expression<Func<T, bool>> searchExpression)
    {
        if (!string.IsNullOrEmpty(searchValue))
            return query.Where(searchExpression);
        return query;
    }

    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return await PaginatedList<T>.CreateAsync(query, pageNumber, pageSize);
    }
}

