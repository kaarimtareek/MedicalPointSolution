using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Common
{
    public class PaginatedList<T> : List<T>
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
        private PaginatedList(IList<T> items, int pageNumber, int pageSize, int totalPages)
       {
            PageNumber = pageNumber;
            TotalPages = totalPages;
            PageSize = pageSize;
            AddRange(items);
       }
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var result = await query.Skip( (pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalPages = count == 0 ? 1 : (int)Math.Ceiling(count / (double)pageSize);
            return new PaginatedList<T>(result, pageNumber, pageSize, totalPages);
        }
        public static PaginatedList<T> Create(IList<T> query, int pageNumber, int pageSize, int totalPages)
        {
            return new PaginatedList<T>(query, pageNumber, pageSize, totalPages);
        }
    }
}
