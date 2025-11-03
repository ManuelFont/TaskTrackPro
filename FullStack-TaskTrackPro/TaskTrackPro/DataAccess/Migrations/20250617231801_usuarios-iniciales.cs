using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class usuariosiniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "adminProyecto@mail.com");

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "miembro@mail.com");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "adminSistema@mail.com",
                column: "Password",
                value: "da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "adminSistema@mail.com",
                column: "Password",
                value: "Pass123@");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Email", "Apellido", "FechaNacimiento", "Nombre", "Password", "RolesSerializados" },
                values: new object[,]
                {
                    { "adminProyecto@mail.com", "Proyecto", new DateTime(1998, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Pass123@", "AdminProyecto" },
                    { "miembro@mail.com", "Proyecto", new DateTime(1988, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miembro", "Pass123@", "MiembroProyecto" }
                });
        }
    }
}
