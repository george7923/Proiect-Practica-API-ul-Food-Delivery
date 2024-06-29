namespace PRACTICA_OFICIAL.DTOs
{
    public class ComandaDto
    {
        public string Username { get; set; }
        public string NumeRestaurant { get; set; } // NumeleRestaurantului
        public List<ProdusDto> Produse { get; set; }
        public double? Tips { get; set; } // Added Tips
    }
}
