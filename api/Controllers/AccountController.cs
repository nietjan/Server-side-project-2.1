using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Controllers {
    [Route("api/Account/[action]")]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration) {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult> Login(string email, string password) {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null) {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, password);
                //401 - UnAuthorized - incorrect password
                if (!checkPasswordResult) return Unauthorized(new {
                    StatusCode = 401,
                    Message = "Email or password are incorrect"
                });
            }
            //404 - not found - package does not exist
            else return NotFound(new {
                StatusCode = 404,
                Message = "User not found"
            });

            var token = CreateToken(user);

            //Else return ok
            return Ok(new {
                StatusCode = 200,
                Message = "User logged in successful",
                data = new {
                    bearerToken = token
                }
            });

        }

        private string CreateToken(IdentityUser user) {
            List<Claim> claims = new List<Claim> { 
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            //get key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value!));

            //get credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //create token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            //with handler write token
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
