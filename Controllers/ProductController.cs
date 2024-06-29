using Microsoft.AspNetCore.Mvc;
using PRACTICA_OFICIAL.DataLayer;
using PRACTICA_OFICIAL.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace PRACTICA_OFICIAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DB_Bolt _context;
        private readonly IConfiguration _configuration;

        public ProductController(DB_Bolt context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<ProdusDto>>> GetAllProducts()
        {
            var products = await _context.Produse
                .Include(p => p.Restaurant)
                .ToListAsync();

            var result = products.Select(p => new ProdusDto
            {
                NumeProdus = p.Nume,
                Pret = p.Pret,
                NumeRestaurant = p.Restaurant.Nume
            }).ToList();

            return Ok(result);
        }

        [HttpGet("GetProductByName/{nume}")]
        public async Task<ActionResult<ProdusDto>> GetProductByName(string nume)
        {
            var produs = await _context.Produse
                .Include(p => p.Restaurant)
                .FirstOrDefaultAsync(p => p.Nume == nume);
            if (produs == null)
            {
                return NotFound();
            }

            var result = new ProdusDto
            {
                NumeProdus = produs.Nume,
                Pret = produs.Pret,
                NumeRestaurant = produs.Restaurant.Nume
            };

            return Ok(result);
        }

        [HttpPost("AddProduct")]
        public async Task<ActionResult<Produs>> AddProduct([FromBody] ProdusDto produsDto)
        {
            var restaurant = await _context.Restaurante.SingleOrDefaultAsync(r => r.Nume == produsDto.NumeRestaurant);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            var produs = new Produs
            {
                Nume = produsDto.NumeProdus,
                Pret = produsDto.Pret,
                IdRestaurant = restaurant.IdRestaurant
            };

            _context.Produse.Add(produs);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductByName), new { nume = produs.Nume }, produs);
        }

        [HttpPut("UpdateProduct/{nume}")]
        public async Task<IActionResult> UpdateProduct(string nume, [FromBody] ProdusDto produsDto)
        {
            var produs = await _context.Produse.SingleOrDefaultAsync(p => p.Nume == nume);
            if (produs == null)
            {
                return NotFound("Product not found.");
            }

            var restaurant = await _context.Restaurante.SingleOrDefaultAsync(r => r.Nume == produsDto.NumeRestaurant);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            produs.Nume = produsDto.NumeProdus;
            produs.Pret = produsDto.Pret;
            produs.IdRestaurant = restaurant.IdRestaurant;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Produse.Any(p => p.Nume == nume))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("DeleteProduct/{nume}")]
        public async Task<IActionResult> DeleteProduct(string nume)
        {
            var produs = await _context.Produse.SingleOrDefaultAsync(p => p.Nume == nume);
            if (produs == null)
            {
                return NotFound();
            }

            _context.Produse.Remove(produs);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
