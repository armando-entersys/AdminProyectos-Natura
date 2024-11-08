using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class RelacionesMateriales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audiencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Formato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialMateriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    archivos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialMateriales_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PCN",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCN", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prioridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prioridad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RetrasoMateriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MotivoId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetrasoMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetrasoMateriales_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_AudienciaId",
                table: "Materiales",
                column: "AudienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_FormatoId",
                table: "Materiales",
                column: "FormatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_PCNId",
                table: "Materiales",
                column: "PCNId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_PrioridadId",
                table: "Materiales",
                column: "PrioridadId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMateriales_MaterialId",
                table: "HistorialMateriales",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RetrasoMateriales_MaterialId",
                table: "RetrasoMateriales",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_Audiencia_AudienciaId",
                table: "Materiales",
                column: "AudienciaId",
                principalTable: "Audiencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_Formato_FormatoId",
                table: "Materiales",
                column: "FormatoId",
                principalTable: "Formato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_PCN_PCNId",
                table: "Materiales",
                column: "PCNId",
                principalTable: "PCN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_Prioridad_PrioridadId",
                table: "Materiales",
                column: "PrioridadId",
                principalTable: "Prioridad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_Audiencia_AudienciaId",
                table: "Materiales");

            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_Formato_FormatoId",
                table: "Materiales");

            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_PCN_PCNId",
                table: "Materiales");

            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_Prioridad_PrioridadId",
                table: "Materiales");

            migrationBuilder.DropTable(
                name: "Audiencia");

            migrationBuilder.DropTable(
                name: "Formato");

            migrationBuilder.DropTable(
                name: "HistorialMateriales");

            migrationBuilder.DropTable(
                name: "PCN");

            migrationBuilder.DropTable(
                name: "Prioridad");

            migrationBuilder.DropTable(
                name: "RetrasoMateriales");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_AudienciaId",
                table: "Materiales");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_FormatoId",
                table: "Materiales");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_PCNId",
                table: "Materiales");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_PrioridadId",
                table: "Materiales");
        }
    }
}
