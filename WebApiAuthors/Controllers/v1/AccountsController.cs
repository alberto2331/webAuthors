using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers.v1
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser>  userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signinManager;
        private readonly HashService hashService;
        private readonly IDataProtector dp;

        public AccountsController(
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration, 
            SignInManager<IdentityUser> signinManager,
            IDataProtectionProvider dpp,
            HashService hashService) 
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signinManager = signinManager;
            this.hashService = hashService;
            dp = dpp.CreateProtector("unique_and_random_value_not_shareable_with_anyone");
        }

        [HttpGet("encrypt")]
        public ActionResult encript()
        {
            var textPlano = "Alberto Saiz";
            var textoEncriptado = dp.Protect(textPlano);
            var textoDesencriptado = dp.Unprotect(textoEncriptado);

            return Ok(new
            {
                texto_Plano = textPlano,
                texto_Encriptado = textoEncriptado,
                texto_Desencriptado = textoDesencriptado,
            });
        }
        
        [HttpGet("encryptByTime")]
        public ActionResult encryptByTime()
        {
            var limitedTimeEncryption = dp.ToTimeLimitedDataProtector();

            var textPlano = "Alberto Saiz";
            var textoEncriptado = limitedTimeEncryption.Protect(textPlano, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textoDesencriptado = limitedTimeEncryption.Unprotect(textoEncriptado);

            return Ok(new
            {
                texto_Plano = textPlano,
                texto_Encriptado = textoEncriptado,
                texto_Desencriptado = textoDesencriptado,
            });
        }

        [HttpGet("hash/{planeText}")]
        public ActionResult CreateHash(string planeText)
        {
            var res1 = hashService.Hash(planeText);
            var res2 = hashService.Hash(planeText);
            return Ok(new
            {
                planeText = planeText,
                Hash1 = res1,
                Hash2 = res2,
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AnswerAuthenticationDTO>> Register(UserCredentialsDTO userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return await createToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AnswerAuthenticationDTO>> Login(UserCredentialsDTO userCredentials)
        {
            var result = await signinManager.PasswordSignInAsync(
                userCredentials.Email,
                userCredentials.Password,
                isPersistent: false,
                lockoutOnFailure: false); //Bloquea al usuario si los intentos son no satisfactorios
            if (result.Succeeded)
            {
                return await createToken(userCredentials);
            }
            else
            {
                return BadRequest("Incorrect Login");
            }
        }
        private async Task<AnswerAuthenticationDTO> createToken(UserCredentialsDTO userCredentials)
        {
            var claims = new List<Claim>()
        {
            new Claim("email", userCredentials.Email)
        };
            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            
            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyJwt"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(30);

            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            
            return new AnswerAuthenticationDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration= expiration,
            };
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AnswerAuthenticationDTO>> renewToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var userCredentials = new UserCredentialsDTO
            {
                Email = email,
            };
            return await createToken(userCredentials);
        }

        [HttpPost("MakeAdmin")]
        public async Task<ActionResult> MakeAdmin(EditAdminDTO editAdmin)
        {
            var user = await userManager.FindByEmailAsync(editAdmin.Email);
            //Then we will make the user we just retrieved administrator:
            await userManager.AddClaimAsync(user, new Claim("isAdmin", "1")); //you can put 1 or true or any value
            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdmin)
        {
            var user = await userManager.FindByEmailAsync(editAdmin.Email);
            //Then we will make the user we just retrieved administrator:
            await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1")); //you can put 1 or true or any value
            return NoContent();
        }
    }
}
