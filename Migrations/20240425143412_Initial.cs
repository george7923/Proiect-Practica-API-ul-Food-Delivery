using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRACTICA_OFICIAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cont",
                columns: table => new
                {
                    IdCont = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parola = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cont", x => x.IdCont);
                });

            migrationBuilder.CreateTable(
                name: "Restaurante",
                columns: table => new
                {
                    IdRestaurant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurante", x => x.IdRestaurant);
                });

            migrationBuilder.CreateTable(
                name: "Comenzi",
                columns: table => new
                {
                    IdComanda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumarulProduselor = table.Column<int>(type: "int", nullable: false),
                    IdCont = table.Column<int>(type: "int", nullable: false),
                    IdRestaurant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comenzi", x => x.IdComanda);
                    table.ForeignKey(
                        name: "FK_Comenzi_Cont_IdCont",
                        column: x => x.IdCont,
                        principalTable: "Cont",
                        principalColumn: "IdCont",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comenzi_Restaurante_IdRestaurant",
                        column: x => x.IdRestaurant,
                        principalTable: "Restaurante",
                        principalColumn: "IdRestaurant",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produse",
                columns: table => new
                {
                    IdProdus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdRestaurant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produse", x => x.IdProdus);
                    table.ForeignKey(
                        name: "FK_Produse_Restaurante_IdRestaurant",
                        column: x => x.IdRestaurant,
                        principalTable: "Restaurante",
                        principalColumn: "IdRestaurant",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProduseComandate",
                columns: table => new
                {
                    IdProdusComandat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProdus = table.Column<int>(type: "int", nullable: false),
                    IdComanda = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProduseComandate", x => x.IdProdusComandat);
                    table.ForeignKey(
                        name: "FK_ProduseComandate_Comenzi_IdComanda",
                        column: x => x.IdComanda,
                        principalTable: "Comenzi",
                        principalColumn: "IdComanda",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProduseComandate_Produse_IdProdus",
                        column: x => x.IdProdus,
                        principalTable: "Produse",
                        principalColumn: "IdProdus",
                        onDelete: ReferentialAction.Restrict); // Modificare aici din ReferentialAction.Cascade în ReferentialAction.Restrict
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comenzi_IdCont",
                table: "Comenzi",
                column: "IdCont");

            migrationBuilder.CreateIndex(
                name: "IX_Comenzi_IdRestaurant",
                table: "Comenzi",
                column: "IdRestaurant");

            migrationBuilder.CreateIndex(
                name: "IX_Produse_IdRestaurant",
                table: "Produse",
                column: "IdRestaurant");

            migrationBuilder.CreateIndex(
                name: "IX_ProduseComandate_IdComanda",
                table: "ProduseComandate",
                column: "IdComanda");

            migrationBuilder.CreateIndex(
                name: "IX_ProduseComandate_IdProdus",
                table: "ProduseComandate",
                column: "IdProdus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProduseComandate");

            migrationBuilder.DropTable(
                name: "Comenzi");

            migrationBuilder.DropTable(
                name: "Produse");

            migrationBuilder.DropTable(
                name: "Cont");

            migrationBuilder.DropTable(
                name: "Restaurante");
        }
    }
}
