using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnkormanyzatiPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class KezdetiAdatbazis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bejelentesek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cim = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Leiras = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Helyszin = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LetrehozvaUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kategoria = table.Column<int>(type: "INTEGER", nullable: false),
                    Allapot = table.Column<int>(type: "INTEGER", nullable: false),
                    Hatarido = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Verzio = table.Column<Guid>(type: "TEXT", nullable: false),
                    LezarvaUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Kovetokod = table.Column<Guid>(type: "TEXT", nullable: true),
                    Ertekeles = table.Column<int>(type: "INTEGER", nullable: true),
                    Szelesseg = table.Column<double>(type: "REAL", nullable: true),
                    Hosszusag = table.Column<double>(type: "REAL", nullable: true),
                    Veszelyes = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoBejelentesId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bejelentesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bejelentesek_Bejelentesek_FoBejelentesId",
                        column: x => x.FoBejelentesId,
                        principalTable: "Bejelentesek",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Szervezetek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nev = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Szervezetek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BejelentesNaplok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BejelentesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Muvelet = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ElozoAllapot = table.Column<int>(type: "INTEGER", nullable: true),
                    UjAllapot = table.Column<int>(type: "INTEGER", nullable: false),
                    Hatarido = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdopontUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FelhasznaloId = table.Column<string>(type: "TEXT", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Reszfeladatok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BejelentesId = table.Column<int>(type: "INTEGER", nullable: false),
                    SzervezetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cim = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Hatarido = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Elkeszult = table.Column<bool>(type: "INTEGER", nullable: false),
                    Megjegyzes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "SzervezetiTagok",
                columns: table => new
                {
                    FelhasznaloId = table.Column<string>(type: "TEXT", nullable: false),
                    SzervezetId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SzervezetiTagok", x => x.FelhasznaloId);
                    table.ForeignKey(
                        name: "FK_SzervezetiTagok_AspNetUsers_FelhasznaloId",
                        column: x => x.FelhasznaloId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SzervezetiTagok_Szervezetek_SzervezetId",
                        column: x => x.SzervezetId,
                        principalTable: "Szervezetek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BejelentesFotok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BejelentesId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReszfeladatId = table.Column<int>(type: "INTEGER", nullable: true),
                    Tipus = table.Column<int>(type: "INTEGER", nullable: false),
                    TartalomTipus = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Tartalom = table.Column<byte[]>(type: "BLOB", nullable: false)
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bejelentesek_FoBejelentesId",
                table: "Bejelentesek",
                column: "FoBejelentesId");

            migrationBuilder.CreateIndex(
                name: "IX_Bejelentesek_Kovetokod",
                table: "Bejelentesek",
                column: "Kovetokod",
                unique: true,
                filter: "[Kovetokod] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BejelentesFotok_BejelentesId",
                table: "BejelentesFotok",
                column: "BejelentesId");

            migrationBuilder.CreateIndex(
                name: "IX_BejelentesFotok_ReszfeladatId",
                table: "BejelentesFotok",
                column: "ReszfeladatId");

            migrationBuilder.CreateIndex(
                name: "IX_BejelentesNaplok_BejelentesId",
                table: "BejelentesNaplok",
                column: "BejelentesId");

            migrationBuilder.CreateIndex(
                name: "IX_Reszfeladatok_BejelentesId",
                table: "Reszfeladatok",
                column: "BejelentesId");

            migrationBuilder.CreateIndex(
                name: "IX_Reszfeladatok_SzervezetId",
                table: "Reszfeladatok",
                column: "SzervezetId");

            migrationBuilder.CreateIndex(
                name: "IX_SzervezetiTagok_SzervezetId",
                table: "SzervezetiTagok",
                column: "SzervezetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BejelentesFotok");

            migrationBuilder.DropTable(
                name: "BejelentesNaplok");

            migrationBuilder.DropTable(
                name: "SzervezetiTagok");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Reszfeladatok");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Bejelentesek");

            migrationBuilder.DropTable(
                name: "Szervezetek");
        }
    }
}
