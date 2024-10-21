using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class AddProyectoClasificacionRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClasificacionProyectoId",
                table: "Proyectos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Proyectos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Proyectos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "clasificacionProyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clasificacionProyectos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_ClasificacionProyectoId",
                table: "Proyectos",
                column: "ClasificacionProyectoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_clasificacionProyectos_ClasificacionProyectoId",
                table: "Proyectos",
                column: "ClasificacionProyectoId",
                principalTable: "clasificacionProyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_clasificacionProyectos_ClasificacionProyectoId",
                table: "Proyectos");

            migrationBuilder.DropTable(
                name: "clasificacionProyectos");

            migrationBuilder.DropIndex(
                name: "IX_Proyectos_ClasificacionProyectoId",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "ClasificacionProyectoId",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Proyectos");
        }
    }
}
