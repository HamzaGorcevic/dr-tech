using System.Linq.Expressions;

namespace drTech_backend.Application.Common.Pagination
{
    public static class QueryHelpers
    {
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Expression<Func<T, bool>>? predicate)
        {
            return predicate is null ? query : query.Where(predicate);
        }

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy, bool desc)
        {
            if (string.IsNullOrWhiteSpace(sortBy)) return query;
            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(param, sortBy);
            var lambda = Expression.Lambda(property, param);
            var method = desc ? "OrderByDescending" : "OrderBy";
            var call = Expression.Call(typeof(Queryable), method, new Type[] { typeof(T), property.Type }, query.Expression, Expression.Quote(lambda));
            return query.Provider.CreateQuery<T>(call);
        }

        public static async Task<PagedResult<T>> ApplyPagingAsync<T>(this IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            var total = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return await Task.FromResult(new PagedResult<T>(items, total, page, pageSize));
        }
    }

    public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
}


