using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedStockTimeSeriesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockSnapshots_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "StockSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.AlterColumn<int>(
                name: "TimeSeriesId",
                schema: "db_stock",
                table: "StockSnapshots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries",
                column: "StockSymbol",
                unique: true,
                filter: "[StockSymbol] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_StockSnapshots_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "StockSnapshots",
                column: "TimeSeriesId",
                principalSchema: "db_stock",
                principalTable: "TimeSeries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockSnapshots_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "StockSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.AlterColumn<int>(
                name: "TimeSeriesId",
                schema: "db_stock",
                table: "StockSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries",
                column: "StockSymbol");

            migrationBuilder.AddForeignKey(
                name: "FK_StockSnapshots_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "StockSnapshots",
                column: "TimeSeriesId",
                principalSchema: "db_stock",
                principalTable: "TimeSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
