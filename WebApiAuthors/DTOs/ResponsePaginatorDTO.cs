using System.Collections.Generic;
namespace WebApiAuthors.DTOs
{
    public class ResponsePaginatorDTO<T> where T : class
    {
        public int currentPage { get; set; }
        public int firstRowOnPage { get; set; }
        public int lastRowOnPage { get; set; }
        public int pageCount { get; set; }
        public int pageSize { get; set; }

        public IList<T> results { get; set; }
        public int rowCount { get; set; }
    }
}
