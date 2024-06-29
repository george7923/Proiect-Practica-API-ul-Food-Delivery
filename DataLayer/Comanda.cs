using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PRACTICA_OFICIAL.DataLayer
{
    public class Comanda
    {
        [Key]
        public int IdComanda { get; set; }
        public double? Tips { get; set; }
        public ICollection<Produs> Produse { get; set; } = new List<Produs>();

        [ForeignKey("Cont")]
        public int IdCont { get; set; }
        public Cont Cont { get; set; }

        [ForeignKey("Restaurant")]
        public int IdRestaurant { get; set; }
        public Restaurant Restaurant { get; set; }
    }


}
