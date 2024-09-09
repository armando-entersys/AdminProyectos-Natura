using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class EstatusBreaf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Briefs_TiposBrief_TipoBriefId",
                table: "Briefs");

            migrationBuilder.DropTable(
                name: "TiposBrief");

            migrationBuilder.DropColumn(
                name: "IdEstatusBrief",
                table: "Briefs");

            migrationBuilder.RenameColumn(
                name: "TipoBriefId",
                table: "Briefs",
                newName: "EstatusBriefId");

            migrationBuilder.RenameIndex(
                name: "IX_Briefs_TipoBriefId",
                table: "Briefs",
                newName: "IX_Briefs_EstatusBriefId");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "EstatusBriefs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Briefs_EstatusBriefs_EstatusBriefId",
                table: "Briefs",
                column: "EstatusBriefId",
                principalTable: "EstatusBriefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Briefs_EstatusBriefs_EstatusBriefId",
                table: "Briefs");

            migrationBuilder.RenameColumn(
                name: "EstatusBriefId",
                table: "Briefs",
                newName: "TipoBriefId");

            migrationBuilder.RenameIndex(
                name: "IX_Briefs_EstatusBriefId",
                table: "Briefs",
                newName: "IX_Briefs_TipoBriefId");

            migrationBuilder.AlterColumn<int>(
                name: "Descripcion",
                table: "EstatusBriefs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IdEstatusBrief",
                table: "Briefs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TiposBrief",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposBrief", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Briefs_TiposBrief_TipoBriefId",
                table: "Briefs",
                column: "TipoBriefId",
                principalTable: "TiposBrief",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
