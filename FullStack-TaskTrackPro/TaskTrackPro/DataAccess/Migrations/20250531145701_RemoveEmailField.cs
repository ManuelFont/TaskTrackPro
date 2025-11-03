using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_email",
                table: "Usuarios");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Email", "Apellido", "FechaNacimiento", "Nombre", "Password" },
                values: new object[,]
                {
                    { "adminProyecto@mail.com", "Proyecto", new DateTime(1998, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Pass123@" },
                    { "adminSistema@mail.com", "Sistema", new DateTime(1995, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Pass123@" },
                    { "miembro@mail.com", "Proyecto", new DateTime(1988, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miembro", "Pass123@" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "adminProyecto@mail.com");

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "adminSistema@mail.com");

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Email",
                keyValue: "miembro@mail.com");

            migrationBuilder.AddColumn<string>(
                name: "_email",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
