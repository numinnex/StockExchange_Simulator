using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removedindicessecurities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities");
            migrationBuilder.DropIndex(
                name: "IX_Securities_OrderId",
                schema: "db_stock",
                table: "Securities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Securities_PortfolioId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.DropIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities");

            migrationBuilder.CreateIndex(
                name: "IX_Securities_PortfolioId",
                schema: "db_stock",
                table: "Securities",
                column: "PortfolioId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Securities_StockId",
                schema: "db_stock",
                table: "Securities",
                column: "StockId",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
