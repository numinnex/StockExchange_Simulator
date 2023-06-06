using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedvaluesnapshottable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockPosition",
                schema: "db_stock");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_UserId",
                schema: "db_stock",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "db_stock",
                table: "Portfolios");

            migrationBuilder.CreateTable(
                name: "ValueSnapshots",
                schema: "db_stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Value = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValueSnapshots_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalSchema: "db_stock",
                        principalTable: "Portfolios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_UserId",
                schema: "db_stock",
                table: "Portfolios",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValueSnapshots_PortfolioId",
                schema: "db_stock",
                table: "ValueSnapshots",
                column: "PortfolioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValueSnapshots",
                schema: "db_stock");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_UserId",
                schema: "db_stock",
                table: "Portfolios");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "db_stock",
                table: "Portfolios",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StockPosition",
                schema: "db_stock",
                columns: table => new
                {
                    PortfolioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPosition", x => new { x.PortfolioId, x.Id });
                    table.ForeignKey(
                        name: "FK_StockPosition_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalSchema: "db_stock",
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockPosition_Stocks_StockId",
                        column: x => x.StockId,
                        principalSchema: "db_stock",
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_UserId",
                schema: "db_stock",
                table: "Portfolios",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPosition_StockId",
                schema: "db_stock",
                table: "StockPosition",
                column: "StockId");
        }
    }
}
