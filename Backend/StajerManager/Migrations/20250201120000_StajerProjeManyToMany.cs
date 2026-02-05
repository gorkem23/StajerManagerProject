using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class StajerProjeManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. StajerProjeler tablosunu oluştur
            migrationBuilder.CreateTable(
                name: "StajerProjeler",
                columns: table => new
                {
                    StajerProjeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StajerID = table.Column<int>(type: "int", nullable: false),
                    ProjeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StajerProjeler", x => x.StajerProjeID);
                    table.ForeignKey(
                        name: "FK_StajerProjeler_Stajers_StajerID",
                        column: x => x.StajerID,
                        principalTable: "Stajers",
                        principalColumn: "StajerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StajerProjeler_Projeler_ProjeID",
                        column: x => x.ProjeID,
                        principalTable: "Projeler",
                        principalColumn: "ProjeID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 2. Unique index oluştur (aynı stajer aynı projeye iki kez eklenemez)
            migrationBuilder.CreateIndex(
                name: "IX_StajerProjeler_StajerID_ProjeID",
                table: "StajerProjeler",
                columns: new[] { "StajerID", "ProjeID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StajerProjeler_ProjeID",
                table: "StajerProjeler",
                column: "ProjeID");

            // 3. Mevcut ProjeID verilerini StajerProjeler tablosuna taşı
            migrationBuilder.Sql(@"
                INSERT INTO StajerProjeler (StajerID, ProjeID)
                SELECT StajerID, ProjeID
                FROM Stajers
                WHERE ProjeID IS NOT NULL
            ");

            // 4. Eski foreign key ve index'i kaldır
            migrationBuilder.DropForeignKey(
                name: "FK_Stajers_Projeler_ProjeID",
                table: "Stajers");

            migrationBuilder.DropIndex(
                name: "IX_Stajers_ProjeID",
                table: "Stajers");

            // 5. ProjeID kolonunu Stajers tablosundan kaldır
            migrationBuilder.DropColumn(
                name: "ProjeID",
                table: "Stajers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri alma işlemi
            migrationBuilder.AddColumn<int>(
                name: "ProjeID",
                table: "Stajers",
                type: "int",
                nullable: true);

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

            // Junction table'dan verileri geri taşı (sadece ilk projeyi al)
            migrationBuilder.Sql(@"
                UPDATE Stajers
                SET ProjeID = (
                    SELECT TOP 1 ProjeID
                    FROM StajerProjeler
                    WHERE StajerProjeler.StajerID = Stajers.StajerID
                )
            ");

            migrationBuilder.DropTable(
                name: "StajerProjeler");
        }
    }
}

