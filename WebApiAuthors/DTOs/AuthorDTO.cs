namespace WebApiAuthors.DTOs
{
    public class AuthorDTO:Resource
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Active { get; set; }
        public List<BookDTO> Books { get; set; }

    }
}
