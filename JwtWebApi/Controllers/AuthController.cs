using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       public static User user=new();
       private readonly IConfiguration _configuration;

       public AuthController(IConfiguration configuration)
       {
           _configuration = configuration;
       }

       [HttpPost("register")]
       public ActionResult<User> Register(UserDto request)
       {
           string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
           user.UserName=request.UserName;
           user.PasswordHash=passwordHash;
           return Ok(user);
       }
        //Login
        [HttpPost("login")]
        public ActionResult<User> Login(UserDto request)
        {
            if (user.UserName!=request.UserName)
            {
                return BadRequest("User not found.");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Password not match");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim>claims=new()
            {
                new Claim(ClaimTypes.Name,user.UserName)
            };
            string secret = _configuration.GetSection("AppSettings:Token").Value!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


    }
}
