using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Almacen.Saas.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class TablasDatosFiscalesyTipoPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPFiscal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DireccionFiscal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RFC",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RegimenFiscal",
                table: "Usuarios");

            migrationBuilder.CreateTable(
                name: "RegimenesFiscales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegimenesFiscales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatosFiscales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RFC = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoPersonaFiscal = table.Column<int>(type: "int", nullable: false),
                    Calle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NumExterior = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumInt = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegimenFiscalId = table.Column<int>(type: "int", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosFiscales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatosFiscales_RegimenesFiscales_RegimenFiscalId",
                        column: x => x.RegimenFiscalId,
                        principalTable: "RegimenesFiscales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatosFiscales_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatosFiscales_RegimenFiscalId",
                table: "DatosFiscales",
                column: "RegimenFiscalId");

            migrationBuilder.CreateIndex(
                name: "IX_DatosFiscales_UsuarioId",
                table: "DatosFiscales",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatosFiscales");

            migrationBuilder.DropTable(
                name: "RegimenesFiscales");

            migrationBuilder.AddColumn<string>(
                name: "CPFiscal",
                table: "Usuarios",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionFiscal",
                table: "Usuarios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RFC",
                table: "Usuarios",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Usuarios",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegimenFiscal",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
