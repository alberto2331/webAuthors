using Microsoft.AspNetCore.Identity;

namespace WebApiAuthors.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int BookId { get; set; }
        public Book book { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
