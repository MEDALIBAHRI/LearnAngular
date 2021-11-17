using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int currentPage, int pageSize)
        {
            PageSize = pageSize;
            TotalPages =(int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            PageNumber = currentPage;
            AddRange(items);
        }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber,
        int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count,pageNumber, pageSize);
        }

    }
}