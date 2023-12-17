using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;
using WebAuthorApi.Entity;

namespace WebApiAuthors.Entity
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [FirstCapitalLetter]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<Comment> comments { get; set; }
        public List<AuthorBooks> authorsBooks { get; set; }

    }
}
