using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entity;
using WebApiAuthors.Migrations;
using WebAuthorApi;
using WebAuthorApi.Entity;


namespace WebApiAuthors.Controllers.v1
{
    [ApiController]
    [Route("api/v1/books")]
    //[Authorize]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDTO>> GetById(int id)
        {
            var book = await context.Books
                .Include(book => book.authorsBooks)
                .ThenInclude(authorsBooks => authorsBooks.author)
                .Include(x => x.comments).FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return NotFound("There is no book with that id in the database");
            }

            book.authorsBooks = book.authorsBooks.OrderBy(x => x.Order).ToList();

            var bookDTO = mapper.Map<BookDTO>(book);
            return Ok(bookDTO);
        }

        [HttpPost(Name = "CreateBook")]
        public async Task<ActionResult> Post(AddBookDTO addBookDTO)
        {
            if (addBookDTO.IdsAuthors == null)
            {
                return BadRequest("you cannot create a book without authors");
            }
            var authorsIds = await context.Authors
                .Where(author => addBookDTO.IdsAuthors.Contains(author.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (authorsIds.Count != addBookDTO.IdsAuthors.Count)
            {
                return BadRequest("One of the author or many author doesn´t exist in database");
            }

            var book = mapper.Map<Book>(addBookDTO);

            AssignOrderToAuthor(book);

            context.Add(book);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(int id, AddBookDTO addBookDTO)
        {
            var bookDb = await context.Books.Include(x => x.authorsBooks).FirstOrDefaultAsync(book => book.Id == id);

            if (bookDb == null)
            {
                return NotFound("No books with that id were found in the database");
            }
            bookDb = mapper.Map(addBookDTO, bookDb);

            AssignOrderToAuthor(bookDb);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var bookDb = await context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (bookDb == null)
            {
                return NotFound();
            }

            var bookDto = mapper.Map<BookPatchDto>(bookDb);

            patchDocument.ApplyTo(bookDto, ModelState);

            var isValid = TryValidateModel(bookDto);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            //If everything is fine so:
            mapper.Map(bookDto, bookDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Books.AnyAsync(book => book.Id == id);

            if (!exists)
            {
                return NotFound("There isn´t book with that Id");
            }

            context.Remove(new Book() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

        private void AssignOrderToAuthor(Book book)
        {
            if (book.authorsBooks != null)
            {
                for (int i = 0; i < book.authorsBooks.Count; i++)
                {
                    book.authorsBooks[i].Order = i;
                }
            }
        }
    }
}
