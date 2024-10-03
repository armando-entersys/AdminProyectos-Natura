using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class AgregaTipoBrief : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoBriefId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Briefs_TipoBriefId",
                table: "Briefs",
                column: "TipoBriefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Briefs_TiposBrief_TipoBriefId",
                table: "Briefs",
                column: "TipoBriefId",
                principalTable: "TiposBrief",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Briefs_TiposBrief_TipoBriefId",
                table: "Briefs");

            migrationBuilder.DropTable(
                name: "TiposBrief");

            migrationBuilder.DropIndex(
                name: "IX_Briefs_TipoBriefId",
                table: "Briefs");

            migrationBuilder.DropColumn(
                name: "TipoBriefId",
                table: "Briefs");
        }
    }
}
