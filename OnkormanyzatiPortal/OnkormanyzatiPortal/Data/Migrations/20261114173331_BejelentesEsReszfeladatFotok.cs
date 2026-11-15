using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class BejelentesEsReszfeladatFotok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SzervezetiTagok_AspNetUsers_FelhasznaloId1",
                table: "SzervezetiTagok");

            migrationBuilder.DropIndex(
                name: "IX_SzervezetiTagok_FelhasznaloId1",
                table: "SzervezetiTagok");

            migrationBuilder.DropColumn(
                name: "FelhasznaloId1",
                table: "SzervezetiTagok");

            migrationBuilder.CreateTable(
                name: "BejelentesFotok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BejelentesId = table.Column<int>(type: "int", nullable: false),
                    ReszfeladatId = table.Column<int>(type: "int", nullable: true),
                    Tipus = table.Column<int>(type: "int", nullable: false),
                    TartalomTipus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Tartalom = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BejelentesFotok", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BejelentesFotok_Bejelentesek_BejelentesId",
                        column: x => x.BejelentesId,
                        principalTable: "Bejelentesek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BejelentesFotok_Reszfeladatok_ReszfeladatId",
                        column: x => x.ReszfeladatId,
                        principalTable: "Reszfeladatok",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BejelentesFotok_BejelentesId",
                table: "BejelentesFotok",
                column: "BejelentesId");

            migrationBuilder.CreateIndex(
                name: "IX_BejelentesFotok_ReszfeladatId",
                table: "BejelentesFotok",
                column: "ReszfeladatId");

            migrationBuilder.AddForeignKey(
                name: "FK_SzervezetiTagok_AspNetUsers_FelhasznaloId",
                table: "SzervezetiTagok",
                column: "FelhasznaloId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SzervezetiTagok_AspNetUsers_FelhasznaloId",
                table: "SzervezetiTagok");

            migrationBuilder.DropTable(
                name: "BejelentesFotok");

            migrationBuilder.AddColumn<string>(
                name: "FelhasznaloId1",
                table: "SzervezetiTagok",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SzervezetiTagok_FelhasznaloId1",
                table: "SzervezetiTagok",
                column: "FelhasznaloId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SzervezetiTagok_AspNetUsers_FelhasznaloId1",
                table: "SzervezetiTagok",
                column: "FelhasznaloId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
