using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PRACTICA_OFICIAL.DataLayer
{
    public class Cont
    {
        [Key]
        public int IdCont { get; set; }
        [NotNull]
        public string Username { get; set; }
        [NotNull]
        public string Parola { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
    }


}
