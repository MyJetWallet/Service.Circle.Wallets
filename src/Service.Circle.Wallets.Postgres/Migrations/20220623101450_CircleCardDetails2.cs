using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Circle.Wallets.Postgres.Migrations
{
    public partial class CircleCardDetails2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bin",
                schema: "circle",
                table: "cards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FingerPrint",
                schema: "circle",
                table: "cards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FundingType",
                schema: "circle",
                table: "cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IssuerCountry",
                schema: "circle",
                table: "cards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskEvaluation_Decision",
                schema: "circle",
                table: "cards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskEvaluation_Reason",
                schema: "circle",
                table: "cards",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bin",
                schema: "circle",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "FingerPrint",
                schema: "circle",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "FundingType",
                schema: "circle",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "IssuerCountry",
                schema: "circle",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "RiskEvaluation_Decision",
                schema: "circle",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "RiskEvaluation_Reason",
                schema: "circle",
                table: "cards");
        }
    }
}
