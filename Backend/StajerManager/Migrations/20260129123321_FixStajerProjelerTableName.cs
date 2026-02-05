using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class FixStajerProjelerTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Constraint ve index varsa sil, yoksa atla
            try
            {
                migrationBuilder.Sql(@"
                    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Stajers_ProjeModel_ProjeID')
                    BEGIN
                        ALTER TABLE [Stajers] DROP CONSTRAINT [FK_Stajers_ProjeModel_ProjeID];
                    END
                ");
            }
            catch { }

            try
            {
                migrationBuilder.Sql(@"
                    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Stajers_ProjeID' AND object_id = OBJECT_ID('Stajers'))
                    BEGIN
                        DROP INDEX [IX_Stajers_ProjeID] ON [Stajers];
                    END
                ");
            }
            catch { }

            // ProjeModel tablosu varsa işlemleri yap, yoksa atla
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjeModel')
                BEGIN
                    ALTER TABLE [ProjeModel] DROP CONSTRAINT IF EXISTS [PK_ProjeModel];
                END
            ");

            // Stajers tablosunda ProjeID kolonu varsa sil (önce constraint'i sil)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Stajers_Projeler_ProjeID')
                BEGIN
                    ALTER TABLE [Stajers] DROP CONSTRAINT [FK_Stajers_Projeler_ProjeID];
                END
                
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Stajers') AND name = 'ProjeID')
                BEGIN
                    ALTER TABLE [Stajers] DROP COLUMN [ProjeID];
                END
            ");

            // ProjeModel tablosunu Projeler olarak yeniden adlandır (varsa)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjeModel')
                BEGIN
                    EXEC sp_rename 'ProjeModel', 'Projeler';
                END
            ");

            // DosyaAdi ve DosyaYolu kolonları varsa ekleme
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Projeler') AND name = 'DosyaAdi')
                BEGIN
                    ALTER TABLE [Projeler] ADD [DosyaAdi] nvarchar(255) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Projeler') AND name = 'DosyaYolu')
                BEGIN
                    ALTER TABLE [Projeler] ADD [DosyaYolu] nvarchar(500) NULL;
                END
            ");

            // Primary key varsa ekleme
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Projeler' AND parent_object_id = OBJECT_ID('Projeler'))
                BEGIN
                    ALTER TABLE [Projeler] ADD CONSTRAINT [PK_Projeler] PRIMARY KEY ([ProjeID]);
                END
            ");

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
                        name: "FK_StajerProjeler_Projeler_ProjeID",
                        column: x => x.ProjeID,
                        principalTable: "Projeler",
                        principalColumn: "ProjeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StajerProjeler_Stajers_StajerID",
                        column: x => x.StajerID,
                        principalTable: "Stajers",
                        principalColumn: "StajerID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Index varsa ekleme
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projeler_ProjeAdi' AND object_id = OBJECT_ID('Projeler'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Projeler_ProjeAdi] ON [Projeler] ([ProjeAdi]);
                END
            ");

            migrationBuilder.CreateIndex(
                name: "IX_StajerProjeler_ProjeID",
                table: "StajerProjeler",
                column: "ProjeID");

            migrationBuilder.CreateIndex(
                name: "IX_StajerProjeler_StajerID_ProjeID",
                table: "StajerProjeler",
                columns: new[] { "StajerID", "ProjeID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StajerProjeler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projeler",
                table: "Projeler");

            migrationBuilder.DropIndex(
                name: "IX_Projeler_ProjeAdi",
                table: "Projeler");

            migrationBuilder.DropColumn(
                name: "DosyaAdi",
                table: "Projeler");

            migrationBuilder.DropColumn(
                name: "DosyaYolu",
                table: "Projeler");

            migrationBuilder.RenameTable(
                name: "Projeler",
                newName: "ProjeModel");

            migrationBuilder.AddColumn<int>(
                name: "ProjeID",
                table: "Stajers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjeModel",
                table: "ProjeModel",
                column: "ProjeID");

            migrationBuilder.CreateIndex(
                name: "IX_Stajers_ProjeID",
                table: "Stajers",
                column: "ProjeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Stajers_ProjeModel_ProjeID",
                table: "Stajers",
                column: "ProjeID",
                principalTable: "ProjeModel",
                principalColumn: "ProjeID");
        }
    }
}
