using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class AddDosyaFieldsToProjeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DosyaAdi",
                table: "Projeler",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosyaYolu",
                table: "Projeler",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosyaAdi",
                table: "Projeler");

            migrationBuilder.DropColumn(
                name: "DosyaYolu",
                table: "Projeler");
        }
    }
}

