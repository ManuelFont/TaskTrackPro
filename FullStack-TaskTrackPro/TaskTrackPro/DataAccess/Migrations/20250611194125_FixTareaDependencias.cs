using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixTareaDependencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareaTarea");

            migrationBuilder.CreateTable(
                name: "TareaDependencia",
                columns: table => new
                {
                    DependenciaId = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    TareaId = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaDependencia", x => new { x.DependenciaId, x.TareaId });
                    table.ForeignKey(
                        name: "FK_TareaDependencia_Tarea_DependenciaId",
                        column: x => x.DependenciaId,
                        principalTable: "Tarea",
                        principalColumn: "Titulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareaDependencia_Tarea_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tarea",
                        principalColumn: "Titulo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TareaDependencia_TareaId",
                table: "TareaDependencia",
                column: "TareaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareaDependencia");

            migrationBuilder.CreateTable(
                name: "TareaTarea",
                columns: table => new
                {
                    DependenciasTitulo = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    RequeridoresTitulo = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaTarea", x => new { x.DependenciasTitulo, x.RequeridoresTitulo });
                    table.ForeignKey(
                        name: "FK_TareaTarea_Tarea_DependenciasTitulo",
                        column: x => x.DependenciasTitulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareaTarea_Tarea_RequeridoresTitulo",
                        column: x => x.RequeridoresTitulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TareaTarea_RequeridoresTitulo",
                table: "TareaTarea",
                column: "RequeridoresTitulo");
        }
    }
}
