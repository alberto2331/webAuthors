using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entity;
using WebApiAuthors.Utilities;
using WebAuthorApi;

namespace WebApiAuthors.Controllers.v1
{
    [ApiController]
    [Route("api/v1/books/{bookId:int}/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CommentController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId, [FromQuery] PaginationDTO paginationDTO)
        {
            var bookExists = await context.Books.AnyAsync(book => book.Id == bookId);
            if (!bookExists)
            {
                return NotFound("The id of the book you are looking for does not exist in the database");
            }
            
            var queryable = context.Comments.Where(commentDB => commentDB.BookId == bookId).AsQueryable();
            await HttpContext.InsertPaginationParametersInHeader(queryable);
            var reviews = await queryable.OrderBy(comment => comment.Id).Paged(paginationDTO).ToListAsync();
            
            return mapper.Map<List<CommentDTO>>(reviews);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int bookId, AddCommentDTO commentDto)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var bookExists = await context.Books.AnyAsync(book => book.Id == bookId);
            if (!bookExists)
            {
                return NotFound("The id of the book you are looking for does not exist in the database");
            }
            var comment = mapper.Map<Comment>(commentDto);
            comment.BookId = bookId;
            comment.UserId = userId;
            context.Add(comment);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
