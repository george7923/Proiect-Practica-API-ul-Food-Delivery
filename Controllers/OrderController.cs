using Microsoft.AspNetCore.Mvc;
using PRACTICA_OFICIAL.DataLayer;
using PRACTICA_OFICIAL.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PRACTICA_OFICIAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DB_Bolt _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderController> _logger;

        public OrderController(DB_Bolt context, IConfiguration configuration, ILogger<OrderController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] ComandaDto comandaDto)
        {
            var user = await _context.Cont.SingleOrDefaultAsync(u => u.Username == comandaDto.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var restaurant = await _context.Restaurante.SingleOrDefaultAsync(r => r.Nume == comandaDto.NumeRestaurant);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            var comanda = new Comanda
            {
                IdCont = user.IdCont,
                IdRestaurant = restaurant.IdRestaurant,
                Produse = new List<Produs>(),
                Tips = comandaDto.Tips // Set Tips from DTO
            };

            _context.Comenzi.Add(comanda);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Comanda created with IdComanda: {comanda.IdComanda}");

            foreach (var produsDto in comandaDto.Produse)
            {
                var produs = await _context.Produse.SingleOrDefaultAsync(p => p.Nume == produsDto.NumeProdus && p.IdRestaurant == restaurant.IdRestaurant);
                if (produs != null)
                {
                    var produsComandat = new ProdusComandat
                    {
                        IdComanda = comanda.IdComanda,
                        IdProdus = produs.IdProdus
                    };

                    _context.ProduseComandate.Add(produsComandat);
                    _logger.LogInformation($"ProdusComandat added: IdComanda = {comanda.IdComanda}, IdProdus = {produs.IdProdus}");
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Order placed successfully.", Username = user.Username, NumeRestaurant = restaurant.Nume });
        }

        [HttpPut("ReinitiateOrder/{orderId}")]
        public async Task<IActionResult> ReinitiateOrder(int orderId, [FromBody] ComandaDto updatedOrderDto)
        {
            var order = await _context.Comenzi
                .Include(c => c.Produse)
                .FirstOrDefaultAsync(c => c.IdComanda == orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var user = await _context.Cont.SingleOrDefaultAsync(u => u.Username == updatedOrderDto.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var restaurant = await _context.Restaurante.SingleOrDefaultAsync(r => r.Nume == updatedOrderDto.NumeRestaurant);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            order.IdCont = user.IdCont;
            order.IdRestaurant = restaurant.IdRestaurant;
            order.Tips = updatedOrderDto.Tips; // Update Tips

            // Remove old products
            var oldProducts = _context.ProduseComandate.Where(pc => pc.IdComanda == orderId);
            _context.ProduseComandate.RemoveRange(oldProducts);

            // Add updated products
            foreach (var produsDto in updatedOrderDto.Produse)
            {
                var produs = await _context.Produse.SingleOrDefaultAsync(p => p.Nume == produsDto.NumeProdus && p.IdRestaurant == restaurant.IdRestaurant);
                if (produs != null)
                {
                    var produsComandat = new ProdusComandat
                    {
                        IdComanda = order.IdComanda,
                        IdProdus = produs.IdProdus
                    };

                    _context.ProduseComandate.Add(produsComandat);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Order reinitiated successfully." });
        }

        [HttpGet("GetOrderHistory/{username}")]
        public async Task<ActionResult<IEnumerable<ComandaDto>>> GetOrderHistory(string username)
        {
            var user = await _context.Cont.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var orders = await _context.Comenzi
                .Include(c => c.Restaurant)
                .Where(c => c.IdCont == user.IdCont)
                .ToListAsync();

            var result = new List<ComandaDto>();

            foreach (var order in orders)
            {
                var produseComandate = await _context.ProduseComandate
                    .Where(pc => pc.IdComanda == order.IdComanda)
                    .Include(pc => pc.Produs)
                    .ToListAsync();

                var produseDto = produseComandate.Select(pc => new ProdusDto
                {
                    NumeProdus = pc.Produs.Nume,
                    Pret = pc.Produs.Pret,
                    NumeRestaurant = order.Restaurant.Nume
                }).ToList();

                result.Add(new ComandaDto
                {
                    Username = user.Username,
                    NumeRestaurant = order.Restaurant.Nume,
                    Produse = produseDto,
                    Tips = order.Tips // Include Tips in response
                });
            }

            return Ok(result);
        }

        [HttpPut("AddTip/{orderId}")]
        public async Task<IActionResult> AddTip(int orderId, [FromBody] double? tip)
        {
            if (tip == null || tip < 0)
            {
                return BadRequest("Tip must be a non-negative value.");
            }

            var order = await _context.Comenzi.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.Tips = tip;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error updating the tip.");
            }

            return Ok(new { Message = "Tip updated successfully.", Tips = order.Tips });
        }


        [HttpPost("CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _context.Comenzi.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            _context.Comenzi.Remove(order);

            var produseComandate = await _context.ProduseComandate.Where(pc => pc.IdComanda == orderId).ToListAsync();
            _context.ProduseComandate.RemoveRange(produseComandate);

            await _context.SaveChangesAsync();
            return Ok("Order cancelled successfully.");
        }
    }
}
