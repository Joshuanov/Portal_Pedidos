using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTGU.Migrations
{
    /// <inheritdoc />
    public partial class cecoempresaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CecoEmpresaId",
                table: "CabPedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CecoEmpresaId",
                table: "CabPedidos");
        }
    }
}
