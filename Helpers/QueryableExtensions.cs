using MoviesAPI.DTOs;

namespace MoviesAPI.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.PageRecordsTotal)
                .Take(paginationDTO.PageRecordsTotal);
        }
    }
}
