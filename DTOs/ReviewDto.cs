using System.ComponentModel.DataAnnotations;

namespace PRACTICA_OFICIAL.DTOs
{
    public class ReviewDto
    {
        [Range(1.0, 5.0)]
        public double NumarStele { get; set; } 
        public string? Parere { get; set; }
        public string NumeRestaurant { get; set; }
        public string Username { get; set; }
    }
}
