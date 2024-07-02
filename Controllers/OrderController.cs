using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRACTICA_OFICIAL.DataLayer;
using PRACTICA_OFICIAL.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            Tips = comandaDto.Tips,
            Platit = comandaDto.Platit,
            Livrat = comandaDto.Livrat,
            DataComenzii = comandaDto.DataComenzii == DateTime.MinValue ? DateTime.Now : comandaDto.DataComenzii,
            DataLivrare = comandaDto.DataLivrare == DateTime.MinValue ? DateTime.Now : comandaDto.DataLivrare
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
                    IdProdus = produs.IdProdus,
                    Cantitate = comandaDto.Cantitate
                };

                _context.ProduseComandate.Add(produsComandat);
                _logger.LogInformation($"ProdusComandat added: IdComanda = {comanda.IdComanda}, IdProdus = {produs.IdProdus}, Cantitate = {comandaDto.Cantitate}");
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
        order.Tips = updatedOrderDto.Tips;
        order.Platit = updatedOrderDto.Platit;
        order.Livrat = updatedOrderDto.Livrat;
        order.DataComenzii = updatedOrderDto.DataComenzii == DateTime.MinValue ? order.DataComenzii : updatedOrderDto.DataComenzii;
        order.DataLivrare = updatedOrderDto.DataLivrare == DateTime.MinValue ? order.DataLivrare : updatedOrderDto.DataLivrare;

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
                    IdProdus = produs.IdProdus,
                    Cantitate = updatedOrderDto.Cantitate
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
                Tips = order.Tips,
                Platit = order.Platit,
                Livrat = order.Livrat,
                DataComenzii = order.DataComenzii,
                DataLivrare = order.DataLivrare
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

    [HttpPut("SetPlatit/{orderId}")]
    public async Task<IActionResult> SetPlatit(int orderId, [FromBody] bool platit)
    {
        var order = await _context.Comenzi.FindAsync(orderId);
        if (order == null)
        {
            return NotFound("Order not found.");
        }

        order.Platit = platit;

        

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, "Error updating the payment status.");
        }

        return Ok(new { Message = "Payment status updated successfully.", Platit = order.Platit });
    }

    [HttpPut("SetLivrat/{orderId}")]
    public async Task<IActionResult> SetLivrat(int orderId, [FromBody] bool livrat)
    {
        var order = await _context.Comenzi.FindAsync(orderId);
        if (order == null)
        {
            return NotFound("Order not found.");
        }

        order.Livrat = livrat;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, "Error updating the delivery status.");
        }

        return Ok(new { Message = "Delivery status updated successfully.", Livrat = order.Livrat });
    }

    [HttpGet("EmitereBonFiscal/{orderId}")]
    public async Task<IActionResult> EmitereBonFiscal(int orderId)
    {
        var order = await _context.Comenzi
            .Include(c => c.Produse)
            .Include(c => c.Restaurant)
            .Include(c => c.Cont)
            .FirstOrDefaultAsync(c => c.IdComanda == orderId);

        if (order == null)
        {
            return NotFound("Order not found.");
        }

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

        var comandaDto = new ComandaDto
        {
            Username = order.Cont.Username,
            NumeRestaurant = order.Restaurant.Nume,
            Produse = produseDto,
            Tips = order.Tips,
            Platit = order.Platit,
            Livrat = order.Livrat,
            DataComenzii = order.DataComenzii,
            DataLivrare = order.DataLivrare
        };

        return Ok(comandaDto);
    }
}
