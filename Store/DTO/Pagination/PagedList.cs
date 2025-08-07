using Microsoft.EntityFrameworkCore;

namespace Store.DTO.Pagination
{
    public class PagedList<T>
    {
        public PagedList()
        {
        }

        public PagedList(List<T> items, int count, int pageSize)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        public int TotalPages { get; set; }

        public List<T> Items { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageSize);
        }

    }
    public static class PagedList_Extensions
    {
        public static PagedList<T> ToPagedList<T>(this List<T> source, PaginationParams paginationParams)
        {
            var pageSize = paginationParams.PageSize;
            var count = source.Count();
            var items = source.Skip((paginationParams.PageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageSize);
        }
    }
}
