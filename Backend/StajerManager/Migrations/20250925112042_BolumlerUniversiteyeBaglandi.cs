using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class BolumlerUniversiteyeBaglandi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eski unique index'i kaldır
            migrationBuilder.DropIndex(
                name: "IX_Bolumler_BolumAdi",
                table: "Bolumler");

            // UniversiteID kolonunu ekle
            migrationBuilder.AddColumn<int>(
                name: "UniversiteID",
                table: "Bolumler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Foreign key constraint ekle
            migrationBuilder.AddForeignKey(
                name: "FK_Bolumler_Universiteler_UniversiteID",
                table: "Bolumler",
                column: "UniversiteID",
                principalTable: "Universiteler",
                principalColumn: "UniversiteID",
                onDelete: ReferentialAction.Cascade);

            // Yeni composite unique index ekle
            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_BolumAdi_UniversiteID",
                table: "Bolumler",
                columns: new[] { "BolumAdi", "UniversiteID" },
                unique: true);

            // Foreign key index ekle
            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_UniversiteID",
                table: "Bolumler",
                column: "UniversiteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Foreign key constraint'i kaldır
            migrationBuilder.DropForeignKey(
                name: "FK_Bolumler_Universiteler_UniversiteID",
                table: "Bolumler");

            // Index'leri kaldır
            migrationBuilder.DropIndex(
                name: "IX_Bolumler_BolumAdi_UniversiteID",
                table: "Bolumler");

            migrationBuilder.DropIndex(
                name: "IX_Bolumler_UniversiteID",
                table: "Bolumler");

            // UniversiteID kolonunu kaldır
            migrationBuilder.DropColumn(
                name: "UniversiteID",
                table: "Bolumler");

            // Eski unique index'i geri ekle
            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_BolumAdi",
                table: "Bolumler",
                column: "BolumAdi",
                unique: true);
        }
    }
}
