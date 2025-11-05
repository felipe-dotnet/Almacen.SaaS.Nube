using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Almacen.Saas.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class NuevosCamposFiscalesUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CPFiscal",
                table: "Usuarios",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegimenFiscal",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPFiscal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RegimenFiscal",
                table: "Usuarios");
        }
    }
}
