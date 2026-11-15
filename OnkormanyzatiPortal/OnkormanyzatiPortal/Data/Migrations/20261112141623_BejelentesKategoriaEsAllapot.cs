using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class BejelentesKategoriaEsAllapot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Allapot",
                table: "Bejelentesek",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Kategoria",
                table: "Bejelentesek",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allapot",
                table: "Bejelentesek");

            migrationBuilder.DropColumn(
                name: "Kategoria",
                table: "Bejelentesek");
        }
    }
}
