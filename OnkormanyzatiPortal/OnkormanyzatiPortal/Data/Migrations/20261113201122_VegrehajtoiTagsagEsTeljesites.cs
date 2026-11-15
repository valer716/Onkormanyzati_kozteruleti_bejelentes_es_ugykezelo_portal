using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class VegrehajtoiTagsagEsTeljesites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Megjegyzes",
                table: "Reszfeladatok",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Verzio",
                table: "Bejelentesek",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SzervezetiTagok",
                columns: table => new
                {
                    FelhasznaloId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FelhasznaloId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SzervezetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SzervezetiTagok", x => x.FelhasznaloId);
                    table.ForeignKey(
                        name: "FK_SzervezetiTagok_AspNetUsers_FelhasznaloId1",
                        column: x => x.FelhasznaloId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SzervezetiTagok_Szervezetek_SzervezetId",
                        column: x => x.SzervezetId,
                        principalTable: "Szervezetek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SzervezetiTagok_FelhasznaloId1",
                table: "SzervezetiTagok",
                column: "FelhasznaloId1");

            migrationBuilder.CreateIndex(
                name: "IX_SzervezetiTagok_SzervezetId",
                table: "SzervezetiTagok",
                column: "SzervezetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SzervezetiTagok");

            migrationBuilder.DropColumn(
                name: "Megjegyzes",
                table: "Reszfeladatok");

            migrationBuilder.DropColumn(
                name: "Verzio",
                table: "Bejelentesek");
        }
    }
}
