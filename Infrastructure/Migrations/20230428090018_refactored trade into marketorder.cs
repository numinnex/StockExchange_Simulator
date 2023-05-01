using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class refactoredtradeintomarketorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades",
                schema: "db_stock");

            migrationBuilder.CreateTable(
                name: "MarketTrades",
                schema: "db_stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsBuy = table.Column<bool>(type: "bit", nullable: false),
                    StockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpenQuantity_Value = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    OrderAmount_Value = table.Column<decimal>(type: "money", nullable: true),
                    Price_Value = table.Column<decimal>(type: "money", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TradeCondition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketTrades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_stock",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketTrades_Stocks_StockId",
                        column: x => x.StockId,
                        principalSchema: "db_stock",
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketTrades_StockId",
                schema: "db_stock",
                table: "MarketTrades",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketTrades_UserId",
                schema: "db_stock",
                table: "MarketTrades",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketTrades",
                schema: "db_stock");

            migrationBuilder.CreateTable(
                name: "Trades",
                schema: "db_stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Price_Value = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_stock",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trades_Stocks_StockId",
                        column: x => x.StockId,
                        principalSchema: "db_stock",
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trades_StockId",
                schema: "db_stock",
                table: "Trades",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_UserId",
                schema: "db_stock",
                table: "Trades",
                column: "UserId");
        }
    }
}
