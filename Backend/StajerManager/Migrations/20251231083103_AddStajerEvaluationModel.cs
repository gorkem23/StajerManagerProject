using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajerManager.Migrations
{
    /// <inheritdoc />
    public partial class AddStajerEvaluationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StajerEvaluations",
                columns: table => new
                {
                    EvaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StajerID = table.Column<int>(type: "int", nullable: false),
                    EvaluationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StajerEvaluations", x => x.EvaluationID);
                    table.ForeignKey(
                        name: "FK_StajerEvaluations_Stajers_StajerID",
                        column: x => x.StajerID,
                        principalTable: "Stajers",
                        principalColumn: "StajerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StajerEvaluations_StajerID_EvaluationDate",
                table: "StajerEvaluations",
                columns: new[] { "StajerID", "EvaluationDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StajerEvaluations_StajerID",
                table: "StajerEvaluations",
                column: "StajerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StajerEvaluations");
        }
    }
}
