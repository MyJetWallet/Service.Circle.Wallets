using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Circle.Wallets.Postgres.Migrations
{
    public partial class BankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                schema: "circle",
                table: "cards",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "circle",
                table: "cards",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateTable(
                name: "bank_accounts",
                schema: "circle",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BrokerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BankAccountId = table.Column<string>(type: "text", nullable: true),
                    BankAccountStatus = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TrackingRef = table.Column<string>(type: "text", nullable: true),
                    FingerPrint = table.Column<string>(type: "text", nullable: true),
                    BillingDetailsName = table.Column<string>(type: "text", nullable: true),
                    BillingDetailsCity = table.Column<string>(type: "text", nullable: true),
                    BillingDetailsCountry = table.Column<string>(type: "text", nullable: true),
                    BillingDetailsLine1 = table.Column<string>(type: "text", nullable: true),
                    BillingDetailsLine2 = table.Column<string>(type: "text", nullable: true),
                    BillingDetailsDistrict = table.Column<string>(type: "text", nullable: true),
                    BillingDetailsPostalCode = table.Column<string>(type: "text", nullable: true),
                    BankAddressBankName = table.Column<string>(type: "text", nullable: true),
                    BankAddressCity = table.Column<string>(type: "text", nullable: true),
                    BankAddressCountry = table.Column<string>(type: "text", nullable: true),
                    BankAddressLine1 = table.Column<string>(type: "text", nullable: true),
                    BankAddressLine2 = table.Column<string>(type: "text", nullable: true),
                    BankAddressDistrict = table.Column<string>(type: "text", nullable: true),
                    Error = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Iban = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    RoutingNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_accounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bank_accounts_BankAccountId",
                schema: "circle",
                table: "bank_accounts",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_bank_accounts_BrokerId_ClientId",
                schema: "circle",
                table: "bank_accounts",
                columns: new[] { "BrokerId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_bank_accounts_BrokerId_ClientId_IsActive",
                schema: "circle",
                table: "bank_accounts",
                columns: new[] { "BrokerId", "ClientId", "IsActive" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bank_accounts",
                schema: "circle");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                schema: "circle",
                table: "cards",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "circle",
                table: "cards",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
