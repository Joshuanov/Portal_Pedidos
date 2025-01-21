using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTGU.Migrations
{
    /// <inheritdoc />
    public partial class ActualizaCabPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabPedidos_Cecos_CecoId",
                table: "CabPedidos");

            migrationBuilder.DropTable(
                name: "Cecos");

            migrationBuilder.DropIndex(
                name: "IX_CabPedidos_CecoId",
                table: "CabPedidos");

            migrationBuilder.DropColumn(
                name: "CecoEmpresaId",
                table: "CabPedidos");

            migrationBuilder.DropColumn(
                name: "CecoId",
                table: "CabPedidos");

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "CabPedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdCeco",
                table: "CabPedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "CabPedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "CabPedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "CabPedidos");

            migrationBuilder.DropColumn(
                name: "IdCeco",
                table: "CabPedidos");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "CabPedidos");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "CabPedidos");

            migrationBuilder.AddColumn<int>(
                name: "CecoEmpresaId",
                table: "CabPedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CecoId",
                table: "CabPedidos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Cecos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cecos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CabPedidos_CecoId",
                table: "CabPedidos",
                column: "CecoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CabPedidos_Cecos_CecoId",
                table: "CabPedidos",
                column: "CecoId",
                principalTable: "Cecos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
