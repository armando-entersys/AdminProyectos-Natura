using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class AddAlertaTipoAlertaRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdTipoAlerta",
                table: "Alertas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TipoAlerta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoAlerta", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_IdTipoAlerta",
                table: "Alertas",
                column: "IdTipoAlerta");

            migrationBuilder.AddForeignKey(
                name: "FK_Alertas_TipoAlerta_IdTipoAlerta",
                table: "Alertas",
                column: "IdTipoAlerta",
                principalTable: "TipoAlerta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertas_TipoAlerta_IdTipoAlerta",
                table: "Alertas");

            migrationBuilder.DropTable(
                name: "TipoAlerta");

            migrationBuilder.DropIndex(
                name: "IX_Alertas_IdTipoAlerta",
                table: "Alertas");

            migrationBuilder.DropColumn(
                name: "IdTipoAlerta",
                table: "Alertas");
        }
    }
}
