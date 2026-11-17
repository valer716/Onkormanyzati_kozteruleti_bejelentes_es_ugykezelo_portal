using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class KovetokodSzamkent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bejelentesek_Kovetokod",
                table: "Bejelentesek");

            migrationBuilder.DropColumn(
                name: "Kovetokod",
                table: "Bejelentesek");

            migrationBuilder.AddColumn<string>(
                name: "Kovetokod",
                table: "Bejelentesek",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE b
SET Kovetokod = CAST(100000 + (n.Sorszam * 7907) % 900000 AS nvarchar(6))
FROM Bejelentesek b
JOIN (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS Sorszam FROM Bejelentesek) n ON n.Id = b.Id");

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
                name: "Kovetokod",
                table: "Bejelentesek");

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
    }
}
