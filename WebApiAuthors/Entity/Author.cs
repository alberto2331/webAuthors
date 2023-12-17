using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Entity;
using WebApiAuthors.Validations;

namespace WebAuthorApi.Entity
{
    public class Author
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public String Active { get; set; }
        //public List<Book> books { get; set; }
        public List<AuthorBooks> authorsBooks { get; set; }
    }
}
