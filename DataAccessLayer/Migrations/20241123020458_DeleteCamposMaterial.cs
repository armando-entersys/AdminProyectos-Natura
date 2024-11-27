using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class DeleteCamposMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Proceso",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "Produccion",
                table: "Materiales");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Proceso",
                table: "Materiales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Produccion",
                table: "Materiales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
