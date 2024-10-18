using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class CrearRelacionUnoAUnoProyectoBrief : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Material_Briefs_BriefId",
                table: "Material");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Material",
                table: "Material");

            migrationBuilder.RenameTable(
                name: "Material",
                newName: "Materiales");

            migrationBuilder.RenameIndex(
                name: "IX_Material_BriefId",
                table: "Materiales",
                newName: "IX_Materiales_BriefId");

            migrationBuilder.AlterColumn<string>(
                name: "Contrasena",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Materiales",
                table: "Materiales",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Proyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BriefId = table.Column<int>(type: "int", nullable: false),
                    ClasificaciónId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequierePlan = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyectos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proyectos_Briefs_BriefId",
                        column: x => x.BriefId,
                        principalTable: "Briefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_BriefId",
                table: "Proyectos",
                column: "BriefId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_Briefs_BriefId",
                table: "Materiales",
                column: "BriefId",
                principalTable: "Briefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_Briefs_BriefId",
                table: "Materiales");

            migrationBuilder.DropTable(
                name: "Proyectos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Materiales",
                table: "Materiales");

            migrationBuilder.RenameTable(
                name: "Materiales",
                newName: "Material");

            migrationBuilder.RenameIndex(
                name: "IX_Materiales_BriefId",
                table: "Material",
                newName: "IX_Material_BriefId");

            migrationBuilder.AlterColumn<string>(
                name: "Contrasena",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Material",
                table: "Material",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Material_Briefs_BriefId",
                table: "Material",
                column: "BriefId",
                principalTable: "Briefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
