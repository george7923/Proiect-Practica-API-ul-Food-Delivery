using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PRACTICA_OFICIAL.DataLayer
{
    public class ProdusComandat
    {
        [Key]
        public int IdProdusComandat { get; set; }

        [ForeignKey("Comanda")]
        public int IdComanda { get; set; }
        public Comanda Comanda { get; set; }
        [ForeignKey("Produs")]
        public int IdProdus {  get; set; }
        public Produs Produs { get; set; }
    }
}
