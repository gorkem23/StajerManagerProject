using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class AddProjeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjeID",
                table: "Stajers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Projeler",
                columns: table => new
                {
                    ProjeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjeAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projeler", x => x.ProjeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projeler_ProjeAdi",
                table: "Projeler",
                column: "ProjeAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stajers_ProjeID",
                table: "Stajers",
                column: "ProjeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Stajers_Projeler_ProjeID",
                table: "Stajers",
                column: "ProjeID",
                principalTable: "Projeler",
                principalColumn: "ProjeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stajers_Projeler_ProjeID",
                table: "Stajers");

            migrationBuilder.DropTable(
                name: "Projeler");

            migrationBuilder.DropIndex(
                name: "IX_Stajers_ProjeID",
                table: "Stajers");

            migrationBuilder.DropColumn(
                name: "ProjeID",
                table: "Stajers");
        }
    }
}
