using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TareaRecurso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareaRecurso");
            
            migrationBuilder.CreateTable(
                name: "TareaRecurso",
                columns: table => new
                {
                    RecursosId = table.Column<int>(type: "int", nullable: false),
                    TareasTitulo = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaRecurso", x => new { x.RecursosId, x.TareasTitulo });
                    table.ForeignKey(
                        name: "FK_TareaRecurso_Recurso_RecursosId",
                        column: x => x.RecursosId,
                        principalTable: "Recurso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareaRecurso_Tarea_TareasTitulo",
                        column: x => x.TareasTitulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TareaRecurso_TareasTitulo",
                table: "TareaRecurso",
                column: "TareasTitulo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareaRecurso");

            migrationBuilder.CreateTable(
                name: "Tarea_Recurso",
                columns: table => new
                {
                    RecursosId = table.Column<int>(type: "int", nullable: false),
                    TareaTitulo = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarea_Recurso", x => new { x.RecursosId, x.TareaTitulo });
                    table.ForeignKey(
                        name: "FK_TareaRecurso_Recurso_RecursosId",
                        column: x => x.RecursosId,
                        principalTable: "Recurso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareaRecurso_Tarea_TareaTitulo",
                        column: x => x.TareaTitulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tarea_Recurso_TareaTitulo",
                table: "Tarea_Recurso",
                column: "TareaTitulo");
        }
    }
}