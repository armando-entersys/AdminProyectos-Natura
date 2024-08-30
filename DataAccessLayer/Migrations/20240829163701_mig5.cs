using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mig5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdRol",
                table: "Menus",
                newName: "RolId");

            migrationBuilder.AddColumn<int>(
                name: "TipoUsuarioId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TipoUsuarioId",
                table: "Usuarios",
                column: "TipoUsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_RolId",
                table: "Menus",
                column: "RolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Roles_RolId",
                table: "Menus",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_TiposUsuario_TipoUsuarioId",
                table: "Usuarios",
                column: "TipoUsuarioId",
                principalTable: "TiposUsuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Roles_RolId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_TiposUsuario_TipoUsuarioId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_TipoUsuarioId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Menus_RolId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "TipoUsuarioId",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "RolId",
                table: "Menus",
                newName: "IdRol");
        }
    }
}
