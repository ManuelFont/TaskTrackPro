using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class agregar_fk_tarea_en_rangofecha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TareaTitulo",
                table: "RangosFecha",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RangosFecha_TareaTitulo",
                table: "RangosFecha",
                column: "TareaTitulo");

            migrationBuilder.AddForeignKey(
                name: "FK_RangosFecha_Tarea_TareaTitulo",
                table: "RangosFecha",
                column: "TareaTitulo",
                principalTable: "Tarea",
                principalColumn: "Titulo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RangosFecha_Tarea_TareaTitulo",
                table: "RangosFecha");

            migrationBuilder.DropIndex(
                name: "IX_RangosFecha_TareaTitulo",
                table: "RangosFecha");

            migrationBuilder.DropColumn(
                name: "TareaTitulo",
                table: "RangosFecha");
        }
    }
}
