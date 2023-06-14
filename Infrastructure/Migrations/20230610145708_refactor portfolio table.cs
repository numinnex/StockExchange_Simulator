using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class refactorportfoliotable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Securities_AspNetUsers_UserId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropForeignKey(
                name: "FK_Securities_Stocks_StockId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropIndex(
                name: "IX_Securities_UserId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.AlterColumn<Guid>(
                name: "StockId",
                schema: "db_stock",
                table: "Securities",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                schema: "db_stock",
                table: "Securities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PortfolioId",
                schema: "db_stock",
                table: "Securities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                schema: "db_stock",
                table: "Securities",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "PortfolioId",
                schema: "db_stock",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Securities_OrderId",
                schema: "db_stock",
                table: "Securities",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Securities_MarketOrders_OrderId",
                schema: "db_stock",
                table: "Securities",
                column: "OrderId",
                principalSchema: "db_stock",
                principalTable: "MarketOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Securities_Portfolios_PortfolioId",
                schema: "db_stock",
                table: "Securities",
                column: "PortfolioId",
                principalSchema: "db_stock",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                    name: "FK_Securities_Stocks_StockId",
                schema: "db_stock",
                table: "Securities",
                column: "StockId",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Securities_MarketOrders_OrderId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropForeignKey(
                name: "FK_Securities_Portfolios_PortfolioId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropForeignKey(
                name: "FK_Securities_Stocks_StockId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropIndex(
                name: "IX_Securities_OrderId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropIndex(
                name: "IX_Securities_PortfolioId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropColumn(
                name: "PortfolioId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropColumn(
                name: "PortfolioId",
                schema: "db_stock",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "StockId",
                schema: "db_stock",
                table: "Securities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "db_stock",
                table: "Securities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities",
                column: "StockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Securities_UserId",
                schema: "db_stock",
                table: "Securities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Securities_AspNetUsers_UserId",
                schema: "db_stock",
                table: "Securities",
                column: "UserId",
                principalSchema: "db_stock",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Securities_Stocks_StockId",
                schema: "db_stock",
                table: "Securities",
                column: "StockId",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
