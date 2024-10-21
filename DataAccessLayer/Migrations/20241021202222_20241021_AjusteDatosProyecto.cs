using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class _20241021_AjusteDatosProyecto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClasificaciónId",
                table: "Proyectos");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Materiales",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Materiales",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FormatoId",
                table: "Materiales",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "FormatoId",
                table: "Materiales");

            migrationBuilder.AddColumn<int>(
                name: "ClasificaciónId",
                table: "Proyectos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
