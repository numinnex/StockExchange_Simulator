using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedStopOrderstableandfixedmarketorders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketTrades_AspNetUsers_UserId",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketTrades_Fees_FeeId",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketTrades_Stocks_StockId",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketTrades",
                schema: "db_stock",
                table: "MarketTrades");

            migrationBuilder.RenameTable(
                name: "MarketTrades",
                schema: "db_stock",
                newName: "MarketOrders",
                newSchema: "db_stock");

            migrationBuilder.RenameIndex(
                name: "IX_MarketTrades_UserId",
                schema: "db_stock",
                table: "MarketOrders",
                newName: "IX_MarketOrders_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketTrades_StockId",
                schema: "db_stock",
                table: "MarketOrders",
                newName: "IX_MarketOrders_StockId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketTrades_FeeId",
                schema: "db_stock",
                table: "MarketOrders",
                newName: "IX_MarketOrders_FeeId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Timestamp",
                schema: "db_stock",
                table: "MarketOrders",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketOrders",
                schema: "db_stock",
                table: "MarketOrders",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "StopOrders",
                schema: "db_stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsBuy = table.Column<bool>(type: "bit", nullable: false),
                    IsTriggered = table.Column<bool>(type: "bit", nullable: false),
                    StockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StopPrice_Value = table.Column<decimal>(type: "money", nullable: false),
                    OpenQuantity_Value = table.Column<decimal>(type: "money", nullable: true),
                    FeeAmount_Value = table.Column<decimal>(type: "money", nullable: false),
                    Cost_Value = table.Column<decimal>(type: "money", nullable: false),
                    FeeId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StopOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_stock",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StopOrders_Fees_FeeId",
                        column: x => x.FeeId,
                        principalSchema: "db_stock",
                        principalTable: "Fees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StopOrders_Stocks_StockId",
                        column: x => x.StockId,
                        principalSchema: "db_stock",
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StopOrders_FeeId",
                schema: "db_stock",
                table: "StopOrders",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StopOrders_StockId",
                schema: "db_stock",
                table: "StopOrders",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_StopOrders_UserId",
                schema: "db_stock",
                table: "StopOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketOrders_AspNetUsers_UserId",
                schema: "db_stock",
                table: "MarketOrders",
                column: "UserId",
                principalSchema: "db_stock",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketOrders_Fees_FeeId",
                schema: "db_stock",
                table: "MarketOrders",
                column: "FeeId",
                principalSchema: "db_stock",
                principalTable: "Fees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketOrders_Stocks_StockId",
                schema: "db_stock",
                table: "MarketOrders",
                column: "StockId",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketOrders_AspNetUsers_UserId",
                schema: "db_stock",
                table: "MarketOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketOrders_Fees_FeeId",
                schema: "db_stock",
                table: "MarketOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketOrders_Stocks_StockId",
                schema: "db_stock",
                table: "MarketOrders");

            migrationBuilder.DropTable(
                name: "StopOrders",
                schema: "db_stock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketOrders",
                schema: "db_stock",
                table: "MarketOrders");

            migrationBuilder.RenameTable(
                name: "MarketOrders",
                schema: "db_stock",
                newName: "MarketTrades",
                newSchema: "db_stock");

            migrationBuilder.RenameIndex(
                name: "IX_MarketOrders_UserId",
                schema: "db_stock",
                table: "MarketTrades",
                newName: "IX_MarketTrades_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketOrders_StockId",
                schema: "db_stock",
                table: "MarketTrades",
                newName: "IX_MarketTrades_StockId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketOrders_FeeId",
                schema: "db_stock",
                table: "MarketTrades",
                newName: "IX_MarketTrades_FeeId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                schema: "db_stock",
                table: "MarketTrades",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketTrades",
                schema: "db_stock",
                table: "MarketTrades",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTrades_AspNetUsers_UserId",
                schema: "db_stock",
                table: "MarketTrades",
                column: "UserId",
                principalSchema: "db_stock",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTrades_Fees_FeeId",
                schema: "db_stock",
                table: "MarketTrades",
                column: "FeeId",
                principalSchema: "db_stock",
                principalTable: "Fees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTrades_Stocks_StockId",
                schema: "db_stock",
                table: "MarketTrades",
                column: "StockId",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
