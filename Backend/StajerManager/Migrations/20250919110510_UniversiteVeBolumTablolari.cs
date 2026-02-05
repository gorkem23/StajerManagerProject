using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class UniversiteVeBolumTablolari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "University",
                table: "Stajers");

            migrationBuilder.AddColumn<int>(
                name: "BolumID",
                table: "Stajers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniversiteID",
                table: "Stajers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Universiteler",
                columns: table => new
                {
                    UniversiteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversiteAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sehir = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostaKodu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universiteler", x => x.UniversiteID);
                });

            migrationBuilder.CreateTable(
                name: "Bolumler",
                columns: table => new
                {
                    BolumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BolumAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BolumKodu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Fakulte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EgitimSuresi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EgitimTuru = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    UniversiteID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolumler", x => x.BolumID);
                    table.ForeignKey(
                        name: "FK_Bolumler_Universiteler_UniversiteID",
                        column: x => x.UniversiteID,
                        principalTable: "Universiteler",
                        principalColumn: "UniversiteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stajers_BolumID",
                table: "Stajers",
                column: "BolumID");

            migrationBuilder.CreateIndex(
                name: "IX_Stajers_UniversiteID",
                table: "Stajers",
                column: "UniversiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_BolumAdi_UniversiteID",
                table: "Bolumler",
                columns: new[] { "BolumAdi", "UniversiteID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_UniversiteID",
                table: "Bolumler",
                column: "UniversiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Universiteler_UniversiteAdi",
                table: "Universiteler",
                column: "UniversiteAdi",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stajers_Bolumler_BolumID",
                table: "Stajers",
                column: "BolumID",
                principalTable: "Bolumler",
                principalColumn: "BolumID");

            migrationBuilder.AddForeignKey(
                name: "FK_Stajers_Universiteler_UniversiteID",
                table: "Stajers",
                column: "UniversiteID",
                principalTable: "Universiteler",
                principalColumn: "UniversiteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stajers_Bolumler_BolumID",
                table: "Stajers");

            migrationBuilder.DropForeignKey(
                name: "FK_Stajers_Universiteler_UniversiteID",
                table: "Stajers");

            migrationBuilder.DropTable(
                name: "Bolumler");

            migrationBuilder.DropTable(
                name: "Universiteler");

            migrationBuilder.DropIndex(
                name: "IX_Stajers_BolumID",
                table: "Stajers");

            migrationBuilder.DropIndex(
                name: "IX_Stajers_UniversiteID",
                table: "Stajers");

            migrationBuilder.DropColumn(
                name: "BolumID",
                table: "Stajers");

            migrationBuilder.DropColumn(
                name: "UniversiteID",
                table: "Stajers");

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "Stajers",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");
        }
    }
}
