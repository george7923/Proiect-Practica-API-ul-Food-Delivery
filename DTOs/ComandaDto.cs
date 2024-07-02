using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRACTICA_OFICIAL.DTOs
{
    public class ComandaDto
    {
        public string Username { get; set; }
        public string NumeRestaurant { get; set; }
        public List<ProdusDto> Produse { get; set; }
        public double? Tips { get; set; }
        public int? Cantitate { get; set; }
        public bool Platit { get; set; }
        public bool Livrat { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DataComenzii { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DataLivrare { get; set; }
    }


}
