using Microsoft.EntityFrameworkCore;
using Store.DTO.Pagination;

namespace Store.Extensions
{
    public static class PaginationExtension
    {
        public static async Task<PagedList<T>> CreateAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageSize);
        }
        public static async Task<PagedList<T>> CreateAsync<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageSize);
        }
    }
}
