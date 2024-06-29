using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PRACTICA_OFICIAL.DataLayer
{
    public class Produs
    {
        [Key]
        public int IdProdus { get; set; }
        [NotNull]
        public string Nume { get; set; }
        public decimal Pret { get; set; }
        public Restaurant Restaurant { get; set; }

        [ForeignKey("Restaurant")]
        public int IdRestaurant { get; set; }

    }
}
