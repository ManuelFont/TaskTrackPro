using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RelacionTareaRecursos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recurso_Tarea_TareaEF_Titulo",
                table: "Recurso");

            migrationBuilder.DropIndex(
                name: "IX_Recurso_TareaEF_Titulo",
                table: "Recurso");

            migrationBuilder.DropColumn(
                name: "TareaEF_Titulo",
                table: "Recurso");

            migrationBuilder.CreateTable(
                name: "TareaRecurso",
                columns: table => new
                {
                    RecursosId = table.Column<int>(type: "int", nullable: false),
                    TareasEF_Titulo = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaRecurso", x => new { x.RecursosId, x.TareasEF_Titulo });
                    table.ForeignKey(
                        name: "FK_TareaRecurso_Recurso_RecursosId",
                        column: x => x.RecursosId,
                        principalTable: "Recurso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareaRecurso_Tarea_TareasEF_Titulo",
                        column: x => x.TareasEF_Titulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TareaRecurso_TareasEF_Titulo",
                table: "TareaRecurso",
                column: "TareasEF_Titulo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareaRecurso");

            migrationBuilder.AddColumn<string>(
                name: "TareaEF_Titulo",
                table: "Recurso",
                type: "nvarchar(200)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recurso_TareaEF_Titulo",
                table: "Recurso",
                column: "TareaEF_Titulo");

            migrationBuilder.AddForeignKey(
                name: "FK_Recurso_Tarea_TareaEF_Titulo",
                table: "Recurso",
                column: "TareaEF_Titulo",
                principalTable: "Tarea",
                principalColumn: "Titulo");
        }
    }
}
