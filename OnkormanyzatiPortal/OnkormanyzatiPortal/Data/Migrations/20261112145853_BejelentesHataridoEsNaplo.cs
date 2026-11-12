using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class BejelentesHataridoEsNaplo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Hatarido",
                table: "Bejelentesek",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BejelentesNaplok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BejelentesId = table.Column<int>(type: "int", nullable: false),
                    Muvelet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ElozoAllapot = table.Column<int>(type: "int", nullable: true),
                    UjAllapot = table.Column<int>(type: "int", nullable: false),
                    Hatarido = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdopontUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FelhasznaloId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BejelentesNaplok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BejelentesNaplok_Bejelentesek_BejelentesId",
                        column: x => x.BejelentesId,
                        principalTable: "Bejelentesek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BejelentesNaplok_BejelentesId",
                table: "BejelentesNaplok",
                column: "BejelentesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BejelentesNaplok");

            migrationBuilder.DropColumn(
                name: "Hatarido",
                table: "Bejelentesek");
        }
    }
}
