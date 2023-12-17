using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;
        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<List<DataHateoas>>> Get()
        {
            var dataHateoas = new List<DataHateoas>();

            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

            dataHateoas.Add(new DataHateoas(
                link: Url.Link("GetRoot", new { }),
                description: "self", //this is because we are pointing to the path the user is on
                method: "GET"));

            dataHateoas.Add(new DataHateoas(
                link: Url.Link("GetAuthors", new { }),
                description: "authors",
                method: "GET"));

            if (isAdmin.Succeeded)
            {
                dataHateoas.Add(new DataHateoas(
                link: Url.Link("CreateAuthor", new { }),
                description: "author-create",
                method: "POST"));

                dataHateoas.Add(new DataHateoas(
                    link: Url.Link("CreateBook", new { }),
                    description: "book-create",
                    method: "POST"));
            }
            return dataHateoas;
        }
    }
}
