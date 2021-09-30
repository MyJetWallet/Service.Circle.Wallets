using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Service.Circle.Wallets.Postgres.Migrations
{
    public partial class InitialCreate : Migration
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrokerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CircleCardId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BillingName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BillingCity = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BillingCountry = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BillingLine1 = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    BillingLine2 = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    BillingDistrict = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    BillingPostalCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExpMonth = table.Column<int>(type: "integer", nullable: false),
                    ExpYear = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SessionId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Network = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Last4 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Bin = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IssuerCountry = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    FundingType = table.Column<int>(type: "integer", nullable: false),
                    Fingerprint = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ErrorCode = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<string>(type: "text", nullable: true),
                    UpdateDate = table.Column<string>(type: "text", nullable: true)
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
                name: "IX_cards_CircleCardId",
                schema: "circle",
                table: "cards",
                column: "CircleCardId",
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
