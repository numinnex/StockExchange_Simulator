using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedStockTimeSeriesKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSeries_Stocks_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Stocks_Symbol",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "StockSymbol",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.AddColumn<int>(
                name: "TimeSeriesId",
                schema: "db_stock",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks",
                column: "TimeSeriesId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks",
                column: "TimeSeriesId",
                principalSchema: "db_stock",
                principalTable: "TimeSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "TimeSeriesId",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.AddColumn<string>(
                name: "StockSymbol",
                schema: "db_stock",
                table: "TimeSeries",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Stocks_Symbol",
                schema: "db_stock",
                table: "Stocks",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries",
                column: "StockSymbol",
                unique: true,
                filter: "[StockSymbol] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSeries_Stocks_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries",
                column: "StockSymbol",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Symbol");
        }
    }
}
