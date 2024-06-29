using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PRACTICA_OFICIAL.DataLayer
{
    public class Restaurant
    {
        [Key]
        public int IdRestaurant { get; set; }
        [NotNull]
        public string Nume { get; set; }
        [NotNull]
        public string Adresa { get; set; }
        public List<Produs> Produse { get; set; }
    }
}
