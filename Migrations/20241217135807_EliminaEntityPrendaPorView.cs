using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTGU.Migrations
{
    /// <inheritdoc />
    public partial class EliminaEntityPrendaPorView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuncionarioId",
                table: "DetPedidos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FuncionarioId",
                table: "DetPedidos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
