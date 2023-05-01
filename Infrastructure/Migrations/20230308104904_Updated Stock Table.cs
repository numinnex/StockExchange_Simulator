using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStockTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSeries_Stocks_StockId",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_StockId",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.DropColumn(
                name: "StockId",
                schema: "db_stock",
                table: "TimeSeries");

            migrationBuilder.RenameColumn(
                name: "Inteval",
                schema: "db_stock",
                table: "TimeSeries",
                newName: "Interval");

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
                column: "StockSymbol");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSeries_Stocks_StockSymbol",
                schema: "db_stock",
                table: "TimeSeries",
                column: "StockSymbol",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Symbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Interval",
                schema: "db_stock",
                table: "TimeSeries",
                newName: "Inteval");

            migrationBuilder.AddColumn<Guid>(
                name: "StockId",
                schema: "db_stock",
                table: "TimeSeries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_StockId",
                schema: "db_stock",
                table: "TimeSeries",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSeries_Stocks_StockId",
                schema: "db_stock",
                table: "TimeSeries",
                column: "StockId",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
