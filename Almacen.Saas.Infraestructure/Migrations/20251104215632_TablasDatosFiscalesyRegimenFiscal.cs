using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Almacen.Saas.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class TablasDatosFiscalesyRegimenFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoPostal",
                table: "DatosFiscales",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoPostal",
                table: "DatosFiscales");
        }
    }
}
