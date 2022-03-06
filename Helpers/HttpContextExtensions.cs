using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPageParameters<T>(this HttpContext httpContext,
            IQueryable<T> queryable, int pageRecords)
        {
            double quantity = await queryable.CountAsync();
            double totalPages = Math.Ceiling(quantity / pageRecords);
            httpContext.Response.Headers.Add("totalPages", totalPages.ToString());
        }
    }
}
