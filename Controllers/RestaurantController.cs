using Microsoft.AspNetCore.Mvc;
using PRACTICA_OFICIAL.DataLayer;
using PRACTICA_OFICIAL.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace PRACTICA_OFICIAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly DB_Bolt _context;
        private readonly IConfiguration _configuration;

        public RestaurantController(DB_Bolt context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<string>>> GetAll()
        {
            var restaurants = await _context.Restaurante
                .Select(r => r.Nume)
                .ToListAsync();

            return Ok(restaurants);
        }

        [HttpGet("GetRestaurantByName/{nume}")]
        public async Task<ActionResult<string>> GetRestaurantByName(string nume)
        {
            var restaurant = await _context.Restaurante
                .FirstOrDefaultAsync(r => r.Nume == nume);
            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant.Nume);
        }


        [HttpPost("AddRestaurant")]
        public async Task<IActionResult> AddRestaurant([FromBody] RestaurantDto restaurantDto)
        {
            var restaurant = new Restaurant
            {
                Nume = restaurantDto.NumeRestaurant,
                Adresa = restaurantDto.Adresa,
                Produse = restaurantDto.Produse.Select(p => new Produs
                {
                    Nume = p.NumeProdus,
                    Pret = p.Pret,
                    IdRestaurant = 0 
                }).ToList()
            };

            _context.Restaurante.Add(restaurant);
            await _context.SaveChangesAsync();

            foreach (var produs in restaurant.Produse)
            {
                produs.IdRestaurant = restaurant.IdRestaurant;
            }

            await _context.SaveChangesAsync();

            return Ok("Restaurant added successfully.");
        }

        [HttpGet("GetMenuByRestaurantName/{restaurantName}")]
        public async Task<ActionResult<IEnumerable<ProdusDto>>> GetMenuByRestaurantName(string restaurantName)
        {
            var restaurant = await _context.Restaurante
                .Include(r => r.Produse)
                .FirstOrDefaultAsync(r => r.Nume == restaurantName);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            var result = restaurant.Produse.Select(p => new ProdusDto
            {
                NumeProdus = p.Nume,
                Pret = p.Pret,
                NumeRestaurant = restaurant.Nume
            }).ToList();

            return Ok(result);
        }
    }
}
