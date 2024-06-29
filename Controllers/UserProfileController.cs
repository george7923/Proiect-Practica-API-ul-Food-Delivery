using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRACTICA_OFICIAL.DataLayer;
using PRACTICA_OFICIAL.DTOs;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PRACTICA_OFICIAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserProfileController : ControllerBase
    {
        private readonly DB_Bolt _context;
        private readonly IConfiguration _configuration;

        public UserProfileController(DB_Bolt context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("GetMyProfile/{username}")]
        public async Task<ActionResult<ContDto>> GetMyProfile(string username)
        {
            var user = await _context.Cont.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            var result = new ContDto
            {
                Username = user.Username,
                Parola = new string('*', user.Parola.Length),
                Email = user.Email,
                Telefon = user.Telefon,
                Adresa = user.Adresa
            };

            return Ok(result);
        }

        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ContDto updatedUserDto)
        {
            var user = await _context.Cont.SingleOrDefaultAsync(u => u.Username == updatedUserDto.Username);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = updatedUserDto.Email;
            user.Telefon = updatedUserDto.Telefon;
            user.Adresa = updatedUserDto.Adresa;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Profile updated successfully" });
        }

        [HttpPost("AddUserAddress")]
        public async Task<IActionResult> AddUserAddress([FromBody] ContDto updatedUserDto)
        {
            var user = await _context.Cont.SingleOrDefaultAsync(u => u.Username == updatedUserDto.Username);
            if (user == null)
            {
                return NotFound();
            }

            user.Adresa = updatedUserDto.Adresa;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Address added successfully" });
        }
    }
}
