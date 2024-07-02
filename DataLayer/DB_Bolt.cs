using Microsoft.EntityFrameworkCore;

namespace PRACTICA_OFICIAL.DataLayer
{
    public class DB_Bolt : DbContext
    {
        public DB_Bolt(DbContextOptions<DB_Bolt> options) : base(options) { }

        public DbSet<Cont> Cont { get; set; }
        public DbSet<Comanda> Comenzi { get; set; }
        public DbSet<Restaurant> Restaurante { get; set; }
        public DbSet<Produs> Produse { get; set; }
        public DbSet<ProdusComandat> ProduseComandate { get; set; }
        public DbSet<Review> Reviewuri { get; set; }


    }

}
