using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class BejelentesTerkepiHelye : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Hosszusag",
                table: "Bejelentesek",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Szelesseg",
                table: "Bejelentesek",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hosszusag",
                table: "Bejelentesek");

            migrationBuilder.DropColumn(
                name: "Szelesseg",
                table: "Bejelentesek");
        }
    }
}
