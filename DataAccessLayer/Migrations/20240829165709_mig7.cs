using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mig7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Briefs",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "IdTipoBrief",
                table: "Briefs",
                newName: "TipoBriefId");

            migrationBuilder.CreateIndex(
                name: "IX_Briefs_TipoBriefId",
                table: "Briefs",
                column: "TipoBriefId");

            migrationBuilder.CreateIndex(
                name: "IX_Briefs_UsuarioId",
                table: "Briefs",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Briefs_TiposBrief_TipoBriefId",
                table: "Briefs",
                column: "TipoBriefId",
                principalTable: "TiposBrief",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Briefs_Usuarios_UsuarioId",
                table: "Briefs",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Briefs_TiposBrief_TipoBriefId",
                table: "Briefs");

            migrationBuilder.DropForeignKey(
                name: "FK_Briefs_Usuarios_UsuarioId",
                table: "Briefs");

            migrationBuilder.DropIndex(
                name: "IX_Briefs_TipoBriefId",
                table: "Briefs");

            migrationBuilder.DropIndex(
                name: "IX_Briefs_UsuarioId",
                table: "Briefs");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Briefs",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "TipoBriefId",
                table: "Briefs",
                newName: "IdTipoBrief");
        }
    }
}
