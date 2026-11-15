using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class KulonlegesUgyekKezelese : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FoBejelentesId",
                table: "Bejelentesek",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Veszelyes",
                table: "Bejelentesek",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Bejelentesek_FoBejelentesId",
                table: "Bejelentesek",
                column: "FoBejelentesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bejelentesek_Bejelentesek_FoBejelentesId",
                table: "Bejelentesek",
                column: "FoBejelentesId",
                principalTable: "Bejelentesek",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bejelentesek_Bejelentesek_FoBejelentesId",
                table: "Bejelentesek");

            migrationBuilder.DropIndex(
                name: "IX_Bejelentesek_FoBejelentesId",
                table: "Bejelentesek");

            migrationBuilder.DropColumn(
                name: "FoBejelentesId",
                table: "Bejelentesek");

            migrationBuilder.DropColumn(
                name: "Veszelyes",
                table: "Bejelentesek");
        }
    }
}
