using AutoMapper;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entity;
using WebAuthorApi.Entity;

namespace WebApiAuthors.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddAuthorDTO, Author>();

            CreateMap<BookPatchDto, Book>().ReverseMap();

            CreateMap<Author, AuthorDTO>()
                .ForMember(authorDTO => authorDTO.Books, options => options.MapFrom(MapAuthorsDtoBooks));

            CreateMap<AddBookDTO, Book>()
                .ForMember(Book => Book.authorsBooks,
                options => options.MapFrom(MapAuthorBooks)); //We create the logic to pass from an integer to an Author entity

            CreateMap<Book, BookDTO>()
                .ForMember(bookDTO => bookDTO.Authors, options => options.MapFrom(MapBooksDtoAuthors));
            
            CreateMap<AddCommentDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }
        private List<BookDTO> MapAuthorsDtoBooks(Author author, AuthorDTO authorDTO)
        {
            var result = new List<BookDTO>();
            if (author.authorsBooks == null) { return result; } //This line is used in the case that there are no book associated with the authors
            foreach (var authorAuthor in author.authorsBooks)
            {
                result.Add(new BookDTO()
                {
                    Id = authorAuthor.BookId,
                    Title = authorAuthor.book.Title,
                });
            }
            return result;
        }

        private List<AuthorDTO> MapBooksDtoAuthors(Book book, BookDTO bookDto)
        {
            var result = new List<AuthorDTO>();
            if (book.authorsBooks == null) { return result; } //This line is used in the case that there are no authors associated with the book
            foreach (var authorbook in book.authorsBooks)
            {
                result.Add(new AuthorDTO()
                {
                    ID = authorbook.AuthorId,
                    Name = authorbook.author.Name, //this data is not in the entity "Authors Books" so I have to access "Author" to get the "name"
                    Active = authorbook.author.Active, //this data is not in the entity "Authors Books" so I have to access "Author" to get to "active"
                });
            }
            return result;
        }

        private List<AuthorBooks> MapAuthorBooks(AddBookDTO addBookDto, Book book) 
        {
            var result = new List<AuthorBooks>();
            if (addBookDto.IdsAuthors == null) { return result; }

            foreach ( var authorId in addBookDto.IdsAuthors)
            {
                result.Add(new AuthorBooks() { AuthorId = authorId });
            }
            return result;
        }
          
    }
}
