using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRACTICA_OFICIAL.Migrations
{
    public partial class Modified1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProduseComandate_Produse_IdProdus",
                table: "ProduseComandate");

            migrationBuilder.DropIndex(
                name: "IX_ProduseComandate_IdProdus",
                table: "ProduseComandate");

            migrationBuilder.DropColumn(
                name: "IdProdus",
                table: "ProduseComandate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdProdus",
                table: "ProduseComandate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProduseComandate_IdProdus",
                table: "ProduseComandate",
                column: "IdProdus");

            migrationBuilder.AddForeignKey(
                name: "FK_ProduseComandate_Produse_IdProdus",
                table: "ProduseComandate",
                column: "IdProdus",
                principalTable: "Produse",
                principalColumn: "IdProdus",
                onDelete: ReferentialAction.Restrict); // Modificați din ReferentialAction.Cascade în ReferentialAction.Restrict

        }
    }
}
