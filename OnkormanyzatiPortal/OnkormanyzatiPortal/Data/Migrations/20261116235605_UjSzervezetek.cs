using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class UjSzervezetek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Szervezetek",
                columns: new[] { "Id", "Nev" },
                values: new object[,]
                {
                    { 4, "Közterület-felügyelet" },
                    { 5, "Közművek (víz, áram)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Szervezetek",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Szervezetek",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
