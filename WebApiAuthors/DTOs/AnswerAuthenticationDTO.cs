using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.DTOs
{
    public class AnswerAuthenticationDTO
    {
        [Required]
        [EmailAddress]
        public string Token { get; set; }
        [Required]
        public DateTime Expiration { get; set; }
    }
}
