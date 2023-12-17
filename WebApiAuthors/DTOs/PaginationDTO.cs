using AutoMapper.Execution;
using System.Runtime.ConstrainedExecution;

namespace WebApiAuthors.DTOs
{
    public class PaginationDTO
    {
        public int page { get; set; } = 1;
        public int recordsPerPage { get; set; } = 5;
        public int lastRowOnPage { get; set; }
        public string name { get; set; }
        public String active { get; set; }

        public readonly int minimunPage = 1;

        public readonly int maximumNumberOfRecordsPerPage = 50;


        public int RecordsPerPage 
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                recordsPerPage = (value > maximumNumberOfRecordsPerPage) ? maximumNumberOfRecordsPerPage : value ;
            }
        }

    }
}
