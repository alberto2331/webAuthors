using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class AddAuthorDTO
    {
        [Required(ErrorMessage = "Please enter a {0}!!!")]
        [StringLength(maximumLength: 120, ErrorMessage = "The {0} cannot have more than {1} characters")]
        [FirstCapitalLetter]
        public string Name { get; set; }
        public string Active { get; set; }
    }
}
