using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class IsUniqueFs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Stajers_Email",
                table: "Stajers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departmans_DepartmanAdi",
                table: "Departmans",
                column: "DepartmanAdi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stajers_Email",
                table: "Stajers");

            migrationBuilder.DropIndex(
                name: "IX_Departmans_DepartmanAdi",
                table: "Departmans");
        }
    }
}
