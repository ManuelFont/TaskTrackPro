using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proyecto",
                columns: table => new
                {
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    _nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicioEstimada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyecto", x => x.Nombre);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    _email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Tarea",
                columns: table => new
                {
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    _titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Realizada = table.Column<bool>(type: "bit", nullable: false),
                    ProyectoNombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuracionEnDias = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarea", x => x.Titulo);
                    table.ForeignKey(
                        name: "FK_Tarea_Proyecto_ProyectoNombre",
                        column: x => x.ProyectoNombre,
                        principalTable: "Proyecto",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoUsuario",
                columns: table => new
                {
                    ListaProyectosNombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ListaUsuariosEmail = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoUsuario", x => new { x.ListaProyectosNombre, x.ListaUsuariosEmail });
                    table.ForeignKey(
                        name: "FK_ProyectoUsuario_Proyecto_ListaProyectosNombre",
                        column: x => x.ListaProyectosNombre,
                        principalTable: "Proyecto",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyectoUsuario_Usuarios_ListaUsuariosEmail",
                        column: x => x.ListaUsuariosEmail,
                        principalTable: "Usuarios",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recurso",
                columns: table => new
                {
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TareaEF_Titulo = table.Column<string>(type: "nvarchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recurso", x => x.Nombre);
                    table.ForeignKey(
                        name: "FK_Recurso_Tarea_TareaEF_Titulo",
                        column: x => x.TareaEF_Titulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo");
                });

            migrationBuilder.CreateTable(
                name: "TareaTarea",
                columns: table => new
                {
                    DependenciasEF_Titulo = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    RequeridoresEF_Titulo = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaTarea", x => new { x.DependenciasEF_Titulo, x.RequeridoresEF_Titulo });
                    table.ForeignKey(
                        name: "FK_TareaTarea_Tarea_DependenciasEF_Titulo",
                        column: x => x.DependenciasEF_Titulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareaTarea_Tarea_RequeridoresEF_Titulo",
                        column: x => x.RequeridoresEF_Titulo,
                        principalTable: "Tarea",
                        principalColumn: "Titulo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoUsuario_ListaUsuariosEmail",
                table: "ProyectoUsuario",
                column: "ListaUsuariosEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Recurso_TareaEF_Titulo",
                table: "Recurso",
                column: "TareaEF_Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Tarea_ProyectoNombre",
                table: "Tarea",
                column: "ProyectoNombre");

            migrationBuilder.CreateIndex(
                name: "IX_TareaTarea_RequeridoresEF_Titulo",
                table: "TareaTarea",
                column: "RequeridoresEF_Titulo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProyectoUsuario");

            migrationBuilder.DropTable(
                name: "Recurso");

            migrationBuilder.DropTable(
                name: "TareaTarea");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Tarea");

            migrationBuilder.DropTable(
                name: "Proyecto");
        }
    }
}
