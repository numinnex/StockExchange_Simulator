using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTradefootprinttablesomeminorfixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeDetails",
                schema: "db_stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BidFee_Value = table.Column<decimal>(type: "money", nullable: true),
                    AskFee_Value = table.Column<decimal>(type: "money", nullable: true),
                    BidCost_Value = table.Column<decimal>(type: "money", nullable: true),
                    RemainingQuantity_Value = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeFootprints",
                schema: "db_stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessedOrderIsBuy = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestingOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessedOrderUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RestingOrderUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatchPrice_Value = table.Column<decimal>(type: "money", nullable: false),
                    Quantity_Value = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TradeDetailsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeFootprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeFootprints_TradeDetails_TradeDetailsId",
                        column: x => x.TradeDetailsId,
                        principalSchema: "db_stock",
                        principalTable: "TradeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeFootprints_TradeDetailsId",
                schema: "db_stock",
                table: "TradeFootprints",
                column: "TradeDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeFootprints",
                schema: "db_stock");

            migrationBuilder.DropTable(
                name: "TradeDetails",
                schema: "db_stock");
        }
    }
}
