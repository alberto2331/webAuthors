using WebApiAuthors.DTOs;

namespace WebApiAuthors.Utilities
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Paged<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.page ) * paginationDTO.RecordsPerPage)
                .Take(paginationDTO.RecordsPerPage);
        }
    }
}
