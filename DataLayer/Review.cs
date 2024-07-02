using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRACTICA_OFICIAL.DataLayer
{
    [Table("Reviewuri")]
    public class Review
    {
        [Key]
        public int IdReview { get; set; }

        [Range(1.0, 5.0)]
        public double NumarStele { get; set; } 

        public string? Parere { get; set; }

        [ForeignKey("Restaurant")]
        public int IdRestaurant { get; set; }
        public Restaurant Restaurant { get; set; }

        [ForeignKey("Cont")]
        public int IdCont { get; set; }
        public Cont Cont { get; set; }
    }
}
