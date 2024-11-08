using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class AddEstatusMaterialRelationToMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstatusMaterialId",
                table: "Materiales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstatusMateriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstatusMateriales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_EstatusMaterialId",
                table: "Materiales",
                column: "EstatusMaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_EstatusMateriales_EstatusMaterialId",
                table: "Materiales",
                column: "EstatusMaterialId",
                principalTable: "EstatusMateriales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_EstatusMateriales_EstatusMaterialId",
                table: "Materiales");

            migrationBuilder.DropTable(
                name: "EstatusMateriales");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_EstatusMaterialId",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "EstatusMaterialId",
                table: "Materiales");
        }
    }
}
