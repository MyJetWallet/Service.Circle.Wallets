using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Circle.Wallets.Postgres.Migrations
{
    public partial class InitCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "circle");

            migrationBuilder.CreateTable(
                name: "cards",
                schema: "circle",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BrokerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CardName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CircleCardId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Last4 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Network = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExpMonth = table.Column<int>(type: "integer", nullable: true),
                    ExpYear = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorCode = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cards_BrokerId_ClientId",
                schema: "circle",
                table: "cards",
                columns: new[] { "BrokerId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_cards_BrokerId_ClientId_IsActive",
                schema: "circle",
                table: "cards",
                columns: new[] { "BrokerId", "ClientId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_cards_CircleCardId",
                schema: "circle",
                table: "cards",
                column: "CircleCardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cards_Id",
                schema: "circle",
                table: "cards",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cards",
                schema: "circle");
        }
    }
}
