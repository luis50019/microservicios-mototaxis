using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicioTarifas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FareType",
                table: "Fares",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Fares",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FareType",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Fares");
        }
    }
}
