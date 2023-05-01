using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Trade_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trades_AspNetUsers_UserId",
                schema: "db_stock",
                table: "Trades");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "db_stock",
                table: "Trades",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_AspNetUsers_UserId",
                schema: "db_stock",
                table: "Trades",
                column: "UserId",
                principalSchema: "db_stock",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trades_AspNetUsers_UserId",
                schema: "db_stock",
                table: "Trades");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "db_stock",
                table: "Trades",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_AspNetUsers_UserId",
                schema: "db_stock",
                table: "Trades",
                column: "UserId",
                principalSchema: "db_stock",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
