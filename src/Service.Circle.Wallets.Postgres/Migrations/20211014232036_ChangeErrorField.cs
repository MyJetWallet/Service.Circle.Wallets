using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Circle.Wallets.Postgres.Migrations
{
    public partial class ChangeErrorField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorCode",
                schema: "circle",
                table: "cards");

            migrationBuilder.AddColumn<string>(
                name: "Error",
                schema: "circle",
                table: "cards",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                schema: "circle",
                table: "cards");

            migrationBuilder.AddColumn<int>(
                name: "ErrorCode",
                schema: "circle",
                table: "cards",
                type: "integer",
                nullable: true);
        }
    }
}
