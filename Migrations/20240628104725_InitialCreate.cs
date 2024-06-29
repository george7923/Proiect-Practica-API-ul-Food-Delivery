using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRACTICA_OFICIAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumarulProduselor",
                table: "Comenzi");

            migrationBuilder.AddColumn<int>(
                name: "ComandaIdComanda",
                table: "Produse",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produse_ComandaIdComanda",
                table: "Produse",
                column: "ComandaIdComanda");

            migrationBuilder.AddForeignKey(
                name: "FK_Produse_Comenzi_ComandaIdComanda",
                table: "Produse",
                column: "ComandaIdComanda",
                principalTable: "Comenzi",
                principalColumn: "IdComanda");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produse_Comenzi_ComandaIdComanda",
                table: "Produse");

            migrationBuilder.DropIndex(
                name: "IX_Produse_ComandaIdComanda",
                table: "Produse");

            migrationBuilder.DropColumn(
                name: "ComandaIdComanda",
                table: "Produse");

            migrationBuilder.AddColumn<int>(
                name: "NumarulProduselor",
                table: "Comenzi",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
