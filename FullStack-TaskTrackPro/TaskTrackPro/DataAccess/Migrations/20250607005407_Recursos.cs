using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Recursos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                    name: "Id",
                    table: "Recurso",
                    type: "int",
                    nullable: false,
                    defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");
            
            migrationBuilder.DropPrimaryKey(
                name: "PK_Recurso",
                table: "Recurso");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Recurso",
                table: "Recurso",
                column: "Id");
            
            migrationBuilder.AddColumn<string>(
                name: "ProyectoNombre",
                table: "Recurso",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Recurso_Proyecto",
                table: "Recurso",
                column: "ProyectoNombre",
                principalTable: "Proyecto",
                principalColumn: "Nombre",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recurso_Proyecto",
                table: "Recurso");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recurso",
                table: "Recurso");

            migrationBuilder.DropColumn(
                name: "ProyectoNombre",
                table: "Recurso");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Recurso");


            migrationBuilder.AddPrimaryKey(
            name: "PK_Recurso",
            table: "Recurso",
            column: "Nombre");

        }
    }
}
