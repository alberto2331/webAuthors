using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Utilities;
using WebAuthorApi;
using WebAuthorApi.Entity;

namespace WebApiAuthors.Controllers.v2
{
    [ApiController]
    [Route("api/v2/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configurations;
        private readonly IAuthorizationService authorizationService;

        public AuthorsController(ApplicationDbContext context, IMapper mapper, IConfiguration configurations,
            IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.configurations = configurations;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetAuthorsv2")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorDTO>> Get([FromQuery] bool withHateoas = true)
        {
            var author = await context.Authors.ToListAsync();
            var authorsDTO = mapper.Map<List<AuthorDTO>>(author);
            authorsDTO.ForEach(author => author.Name = author.Name.ToUpper());

            if (withHateoas)
            {
                var isAdmin = await authorizationService.AuthorizeAsync(User, "IsAdmin");
                authorsDTO.ForEach(dto => GenerateLink(dto, isAdmin.Succeeded));
                var result = new ResourceCollection<AuthorDTO> { Values = authorsDTO };
                result.Links.Add(new DataHateoas(
                link: Url.Link("GetAuthors", new { }),
                description: "self",
                method: "GET"));
                if (isAdmin.Succeeded)
                {
                    result.Links.Add(new DataHateoas(
                    link: Url.Link("CreateAuthor", new { }),
                    description: "create-Author",
                    method: "POST"));
                }
                return Ok(result);
            }
            return Ok(authorsDTO);
        }

        [HttpGet("{id:int}", Name = "GetAuthorByIdv2")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorDTO>> GetById([FromRoute] int id)
        {
            var author = await context.Authors
                .Include(author => author.authorsBooks)
                .ThenInclude(authorsBooks => authorsBooks.book)

                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound("The Id entered does not correspond to an author in the database");
            }
            var authorDTO = mapper.Map<AuthorDTO>(author);
            var isAdmin = await authorizationService.AuthorizeAsync(User, "IsAdmin");
            GenerateLink(authorDTO, isAdmin.Succeeded);
            return authorDTO;
        }

        private void GenerateLink(AuthorDTO authorDto, bool isAdmin)
        {
            authorDto.Links.Add(new DataHateoas(
                link: Url.Link("GetAuthorById", new { id = authorDto.ID }),
                description: "self",
                method: "GET"));
            if (isAdmin)
            {
                authorDto.Links.Add(new DataHateoas(
                link: Url.Link("UpdateAuthor", new { id = authorDto.ID }),
                description: "update-Author",
                method: "PUT"));
                authorDto.Links.Add(new DataHateoas(
                    link: Url.Link("DeleteAuthor", new { id = authorDto.ID }),
                    description: "delete-Author",
                    method: "DELETE"));
            }
        }

        [HttpGet("{searchedName}", Name = "GetAuthorByNamev2")]
        public async Task<List<AuthorDTO>> GetByName([FromRoute] string searchedName)
        {
            var authors = await context.Authors.Where(a => a.Name.Contains(searchedName)).ToListAsync();
            var authorsDTO = mapper.Map<List<AuthorDTO>>(authors);
            return authorsDTO;
        }

        [HttpPost(Name = "CreateAuthorv2")]
        public async Task<ActionResult> Post(AddAuthorDTO addAuthorDTO)
        {
            var thereIsAnAuthorWithTheSameName = await context.Authors.AnyAsync(x => x.Name == addAuthorDTO.Name);
            if (thereIsAnAuthorWithTheSameName)
            {
                return BadRequest($"There is an author with this name: {addAuthorDTO.Name}");
            }

            var author = mapper.Map<Author>(addAuthorDTO);
            context.Add(author);
            await context.SaveChangesAsync();

            var authorDto = mapper.Map<AuthorDTO>(author);
            return CreatedAtRoute("GetAuthorById", new { id = author.Id }, authorDto);
        }

        [HttpPut("{id:int}", Name = "UpdateAuthorv2")]
        public async Task<ActionResult> Put(AddAuthorDTO authorDto, int id)
        {
            var exists = await context.Authors.AnyAsync(author => author.Id == id);

            if (!exists)
            {
                return NotFound("There isn´t author with that Id");
            }

            var author = mapper.Map<Author>(authorDto);

            author.Id = id;

            context.Update(author);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteAuthorv2")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Authors.AnyAsync(author => author.Id == id);

            if (!exists)
            {
                return NotFound("There isn´t author with that Id");
            }

            context.Remove(new Author() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

        //This endpoint is used to know how many records are in the database:
        [HttpGet("numberOfAuthors")]
        public async Task<int> getNumberOfRecords()
        {
            var amount = await context.Authors.CountAsync();
            return amount;
        }

        [HttpGet("getPaged")]
        public async Task<ResponsePaginatorDTO<AuthorDTO>> getPaged([FromQuery] PaginationDTO paginationDTO)
        {

            var queryable = context.Authors.AsQueryable();//.Where(x=> x.Active =true);
            await HttpContext.InsertPaginationParametersInHeader(queryable);

            var author = await queryable.OrderBy(author => author.Id).Paged(paginationDTO).ToListAsync();
            var authorDTO = mapper.Map<List<AuthorDTO>>(author);

            //Start pager with all necessary front attributes:
            var amountRecordsOnDB = await context.Authors.CountAsync();
            var amountRecordesOnPage = paginationDTO.recordsPerPage;

            //var amountPages = Math.Ceiling(amountRecordsOnDB / amountRecordesOnPage);// cantidad de registros en la base de datos/ pageSize
            var response = new ResponsePaginatorDTO<AuthorDTO>
            {

                currentPage = paginationDTO.page,
                firstRowOnPage = paginationDTO.page * paginationDTO.recordsPerPage - paginationDTO.recordsPerPage + 1,
                lastRowOnPage = paginationDTO.page * paginationDTO.recordsPerPage,
                pageCount = amountRecordsOnDB / amountRecordesOnPage,
                pageSize = paginationDTO.recordsPerPage,
                results = authorDTO,
                rowCount = amountRecordsOnDB,

            };
            //End pager with all necessary front attributes:
            return response;
        }

        [HttpGet("configurations")]
        public ActionResult<string> getConfigurations()
        {
            return configurations["VARIABLE_ENTORNO_1"];
        }
    }
}
