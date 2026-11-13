using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class SzervezetekEsReszfeladatok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Szervezetek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Szervezetek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reszfeladatok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BejelentesId = table.Column<int>(type: "int", nullable: false),
                    SzervezetId = table.Column<int>(type: "int", nullable: false),
                    Cim = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Hatarido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Elkeszult = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reszfeladatok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reszfeladatok_Bejelentesek_BejelentesId",
                        column: x => x.BejelentesId,
                        principalTable: "Bejelentesek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reszfeladatok_Szervezetek_SzervezetId",
                        column: x => x.SzervezetId,
                        principalTable: "Szervezetek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Szervezetek",
                columns: new[] { "Id", "Nev" },
                values: new object[,]
                {
                    { 1, "Útfenntartás" },
                    { 2, "Köztisztaság" },
                    { 3, "Közvilágítás" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reszfeladatok_BejelentesId",
                table: "Reszfeladatok",
                column: "BejelentesId");

            migrationBuilder.CreateIndex(
                name: "IX_Reszfeladatok_SzervezetId",
                table: "Reszfeladatok",
                column: "SzervezetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reszfeladatok");

            migrationBuilder.DropTable(
                name: "Szervezetek");
        }
    }
}
