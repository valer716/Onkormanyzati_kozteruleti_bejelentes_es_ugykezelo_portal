using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class UgykovetesEsErtekeles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ertekeles",
                table: "Bejelentesek",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Kovetokod",
                table: "Bejelentesek",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bejelentesek_Kovetokod",
                table: "Bejelentesek",
                column: "Kovetokod",
                unique: true,
                filter: "[Kovetokod] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bejelentesek_Kovetokod",
                table: "Bejelentesek");

            migrationBuilder.DropColumn(
                name: "Ertekeles",
                table: "Bejelentesek");

            migrationBuilder.DropColumn(
                name: "Kovetokod",
                table: "Bejelentesek");
        }
    }
}
