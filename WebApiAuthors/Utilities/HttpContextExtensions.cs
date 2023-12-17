using Microsoft.EntityFrameworkCore;

namespace WebApiAuthors.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParametersInHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if(httpContext == null) { throw new ArgumentNullException(nameof(httpContext));}
            double amount = await queryable.CountAsync();
            httpContext.Response.Headers.Add("CantidadTotalDeRegistrosEnBaseDeDatos", amount.ToString());
        }
    }
}
