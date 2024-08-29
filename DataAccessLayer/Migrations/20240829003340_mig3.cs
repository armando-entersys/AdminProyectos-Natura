using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "Roles",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   Descripcion = table.Column<string>(type: "nvarchar(20)", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Roles", x => x.Id);
               });
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CambioContrasena = table.Column<bool>(type: "bit", nullable: false),
                    SolicitudRegistro = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
