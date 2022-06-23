using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Circle.Wallets.Postgres.Migrations
{
    public partial class CircleCardDetails3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ErrorCode",
                schema: "circle",
                table: "cards",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorCode",
                schema: "circle",
                table: "cards");
        }
    }
}
