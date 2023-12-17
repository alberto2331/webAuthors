using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Entity;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<CommentDTO> comments { get; set; }
        public List<AuthorDTO> Authors { get; set; } 

    }
}
