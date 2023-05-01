using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addedrefreshtokentable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.AlterColumn<int>(
                name: "TimeSeriesId",
                schema: "db_stock",
                table: "Stocks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "db_stock",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false),
                    Invalidated = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "db_stock",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks",
                column: "TimeSeriesId",
                unique: true,
                filter: "[TimeSeriesId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "db_stock",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks",
                column: "TimeSeriesId",
                principalSchema: "db_stock",
                principalTable: "TimeSeries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_TimeSeries_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "db_stock");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_TimeSeriesId",
                schema: "db_stock",
                table: "Stocks");

            migrationBuilder.AlterColumn<int>(
                name: "TimeSeriesId",
                schema: "db_stock",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
