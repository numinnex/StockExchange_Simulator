using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changedstockidtonotbenullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Securities_Stocks_StockId",
                schema: "db_stock",
                table: "Securities");

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

            migrationBuilder.CreateIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities",
                column: "StockId",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Securities_Stocks_StockId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropIndex(
                name: "IX_Securities_StockId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities",
                column: "StockId",
                unique: true,
                filter: "[StockId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Securities_Stocks_StockId",
                schema: "db_stock",
                table: "Securities",
                column: "StockId",
                principalSchema: "db_stock",
                principalTable: "Stocks",
                principalColumn: "Id");
        }
    }
}
