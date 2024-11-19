using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class UsuarioHistorialMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMateriales_Materiales_MaterialId",
                table: "HistorialMateriales");

            migrationBuilder.AddColumn<int>(
                name: "MaterialId1",
                table: "HistorialMateriales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "HistorialMateriales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMateriales_UsuarioId",
                table: "HistorialMateriales",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMateriales_Materiales_MaterialId",
                table: "HistorialMateriales",
                column: "MaterialId",
                principalTable: "Materiales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMateriales_Materiales_MaterialId1",
                table: "HistorialMateriales",
                column: "MaterialId1",
                principalTable: "Materiales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMateriales_Usuarios_UsuarioId",
                table: "HistorialMateriales",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMateriales_Materiales_MaterialId",
                table: "HistorialMateriales");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMateriales_Materiales_MaterialId1",
                table: "HistorialMateriales");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMateriales_Usuarios_UsuarioId",
                table: "HistorialMateriales");

            migrationBuilder.DropIndex(
                name: "IX_HistorialMateriales_MaterialId1",
                table: "HistorialMateriales");

            migrationBuilder.DropIndex(
                name: "IX_HistorialMateriales_UsuarioId",
                table: "HistorialMateriales");

            migrationBuilder.DropColumn(
                name: "MaterialId1",
                table: "HistorialMateriales");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "HistorialMateriales");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMateriales_Materiales_MaterialId",
                table: "HistorialMateriales",
                column: "MaterialId",
                principalTable: "Materiales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
