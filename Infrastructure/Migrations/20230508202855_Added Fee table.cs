using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFeetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cost_Value",
                schema: "db_stock",
                table: "MarketTrades",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FeeAmount_Value",
                schema: "db_stock",
                table: "MarketTrades",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FeeId",
                schema: "db_stock",
                table: "MarketTrades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                schema: "db_stock",
                table: "MarketTrades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Fees",
                schema: "db_stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MakerFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TakerFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketTrades_FeeId",
                schema: "db_stock",
                table: "MarketTrades",
                column: "FeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTrades_Fees_FeeId",
                schema: "db_stock",
                table: "MarketTrades",
                column: "FeeId",
                principalSchema: "db_stock",
                principalTable: "Fees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketTrades_Fees_FeeId",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropTable(
                name: "Fees",
                schema: "db_stock");

            migrationBuilder.DropIndex(
                name: "IX_MarketTrades_FeeId",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropColumn(
                name: "Cost_Value",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropColumn(
                name: "FeeAmount_Value",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropColumn(
                name: "FeeId",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropColumn(
                name: "Symbol",
                schema: "db_stock",
                table: "MarketTrades");
        }
    }
}
