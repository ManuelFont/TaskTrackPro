using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AgregarLiderProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LiderEmail",
                table: "Proyecto",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_LiderEmail",
                table: "Proyecto",
                column: "LiderEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_Usuarios_LiderEmail",
                table: "Proyecto",
                column: "LiderEmail",
                principalTable: "Usuarios",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Usuarios_LiderEmail",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_LiderEmail",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "LiderEmail",
                table: "Proyecto");
        }
    }
}
