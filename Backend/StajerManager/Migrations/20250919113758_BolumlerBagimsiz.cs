using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class BolumlerBagimsiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bolumler_Universiteler_UniversiteID",
                table: "Bolumler");

            migrationBuilder.DropIndex(
                name: "IX_Bolumler_BolumAdi_UniversiteID",
                table: "Bolumler");

            migrationBuilder.DropIndex(
                name: "IX_Bolumler_UniversiteID",
                table: "Bolumler");

            migrationBuilder.DropColumn(
                name: "UniversiteID",
                table: "Bolumler");

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_BolumAdi",
                table: "Bolumler",
                column: "BolumAdi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bolumler_BolumAdi",
                table: "Bolumler");

            migrationBuilder.AddColumn<int>(
                name: "UniversiteID",
                table: "Bolumler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_BolumAdi_UniversiteID",
                table: "Bolumler",
                columns: new[] { "BolumAdi", "UniversiteID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_UniversiteID",
                table: "Bolumler",
                column: "UniversiteID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bolumler_Universiteler_UniversiteID",
                table: "Bolumler",
                column: "UniversiteID",
                principalTable: "Universiteler",
                principalColumn: "UniversiteID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
