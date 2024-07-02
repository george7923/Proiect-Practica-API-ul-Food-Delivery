using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRACTICA_OFICIAL.DataLayer;
using PRACTICA_OFICIAL.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRACTICA_OFICIAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly DB_Bolt _context;

        public ReviewController(DB_Bolt context)
        {
            _context = context;
        }

        [HttpGet("ObtineToateReviewurile")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            return await _context.Reviewuri
                .Include(r => r.Restaurant)
                .Include(r => r.Cont)
                .Select(r => new ReviewDto
                {
                    NumarStele = r.NumarStele,
                    Parere = r.Parere,
                    NumeRestaurant = r.Restaurant.Nume,
                    Username = r.Cont.Username
                })
                .ToListAsync();
        }

        [HttpGet("{id}ObtineReviewDupaId")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviewuri
                .Include(r => r.Restaurant)
                .Include(r => r.Cont)
                .FirstOrDefaultAsync(r => r.IdReview == id);

            if (review == null)
            {
                return NotFound();
            }

            return new ReviewDto
            {
                NumarStele = review.NumarStele,
                Parere = review.Parere,
                NumeRestaurant = review.Restaurant.Nume,
                Username = review.Cont.Username
            };
        }

        [HttpGet("ByRestaurantName/{restaurantName}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByRestaurantName(string restaurantName)
        {
            var reviews = await _context.Reviewuri
                .Include(r => r.Restaurant)
                .Include(r => r.Cont)
                .Where(r => r.Restaurant.Nume.Contains(restaurantName))
                .Select(r => new ReviewDto
                {
                    NumarStele = r.NumarStele,
                    Parere = r.Parere,
                    NumeRestaurant = r.Restaurant.Nume,
                    Username = r.Cont.Username
                })
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
            {
                return NotFound("No reviews found for this restaurant.");
            }

            return Ok(reviews);
        }

        [HttpGet("GetReviewByUsername/{username}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByUsername(string username)
        {
            var reviews = await _context.Reviewuri
                .Include(r => r.Restaurant)
                .Include(r => r.Cont)
                .Where(r => r.Cont.Username.Contains(username))
                .Select(r => new ReviewDto
                {
                    NumarStele = r.NumarStele,
                    Parere = r.Parere,
                    NumeRestaurant = r.Restaurant.Nume,
                    Username = r.Cont.Username
                })
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
            {
                return NotFound("No reviews found for this user.");
            }

            return Ok(reviews);
        }

        [HttpPost("PosteazaReview")]
        public async Task<ActionResult<Review>> PostReview(ReviewDto reviewDto)
        {
            var cont = await _context.Cont.SingleOrDefaultAsync(c => c.Username == reviewDto.Username);
            var restaurant = await _context.Restaurante.SingleOrDefaultAsync(r => r.Nume == reviewDto.NumeRestaurant);

            if (cont == null)
            {
                return NotFound("User not found.");
            }

            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            var review = new Review
            {
                NumarStele = reviewDto.NumarStele,
                Parere = reviewDto.Parere,
                IdRestaurant = restaurant.IdRestaurant,
                IdCont = cont.IdCont
            };

            _context.Reviewuri.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.IdReview }, review);
        }

        [HttpPut("ActualizeazaUnReview{id}")]
        public async Task<IActionResult> PutReview(int id, ReviewDto reviewDto)
        {
            var cont = await _context.Cont.SingleOrDefaultAsync(c => c.Username == reviewDto.Username);
            var restaurant = await _context.Restaurante.SingleOrDefaultAsync(r => r.Nume == reviewDto.NumeRestaurant);

            if (cont == null)
            {
                return NotFound("User not found.");
            }

            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            var review = await _context.Reviewuri.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            review.NumarStele = reviewDto.NumarStele;
            review.Parere = reviewDto.Parere;
            review.IdRestaurant = restaurant.IdRestaurant;
            review.IdCont = cont.IdCont;

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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

        [HttpDelete("StergeReview{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviewuri.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviewuri.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviewuri.Any(e => e.IdReview == id);
        }
    }
}
