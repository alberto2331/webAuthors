using WebAuthorApi.Entity;

namespace WebApiAuthors.Entity
{
    public class AuthorBooks
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public int Order { get; set; } //This property is used to order the authors of a book in ascending or descending order.
        public Author author { get; set; }
        public Book book { get; set; }
    }
}
