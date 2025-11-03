using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRolesSerializadosAUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RolesSerializados",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "adminProyecto@mail.com",
                column: "RolesSerializados",
                value: "AdminProyecto");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "adminSistema@mail.com",
                column: "RolesSerializados",
                value: "AdminSistema");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "miembro@mail.com",
                column: "RolesSerializados",
                value: "MiembroProyecto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RolesSerializados",
                table: "Usuarios");
        }
    }
}
