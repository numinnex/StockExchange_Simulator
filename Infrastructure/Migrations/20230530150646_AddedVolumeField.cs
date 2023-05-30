using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedVolumeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HighMonth",
                schema: "db_stock",
                table: "Stocks",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LowMonth",
                schema: "db_stock",
                table: "Stocks",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Volume",
                schema: "db_stock",
                table: "Stocks",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighMonth",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "LowMonth",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "Volume",
                schema: "db_stock",
                table: "Stocks");
        }
    }
}
