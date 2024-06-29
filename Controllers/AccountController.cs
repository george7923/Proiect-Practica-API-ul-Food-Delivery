using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRACTICA_OFICIAL.DataLayer;
using PRACTICA_OFICIAL.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PRACTICA_OFICIAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DB_Bolt _context;
        private readonly IConfiguration _configuration;

        public AccountController(DB_Bolt context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] ContDto newUserDto)
        {
            if (_context.Cont.Any(u => u.Username == newUserDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            var newUser = new Cont
            {
                Username = newUserDto.Username,
                Parola = newUserDto.Parola,
                Email = newUserDto.Email,
                Telefon = newUserDto.Telefon,
                Adresa = newUserDto.Adresa
            };

            _context.Cont.Add(newUser);
            await _context.SaveChangesAsync();

            var token = GenerateJSONWebToken(newUser);
            return Ok(new { Token = token, Message = "User registered successfully." });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Cont.SingleOrDefault(u => u.Username == login.Username && u.Parola == login.Parola);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateJSONWebToken(user);
            return Ok(new { Token = token, Message = "Login successful." });
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = _context.Cont.SingleOrDefault(u => u.Username == model.Username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Parola = model.NewPassword;
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }

        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = _context.Cont.SingleOrDefault(u => u.Username == model.Username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            Console.WriteLine("Temporary password: 12345");

            return Ok("A temporary password has been sent to your email.");
        }

        private string GenerateJSONWebToken(Cont userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:BE"],
                audience: _configuration["Jwt:AU"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class ChangePasswordModel
    {
        public string Username { get; set; }
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Username { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Parola { get; set; }
    }
}
