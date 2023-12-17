using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class AddBookDTO
    {
        [FirstCapitalLetter]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Title { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<int> IdsAuthors { get; set; }
    }
}
