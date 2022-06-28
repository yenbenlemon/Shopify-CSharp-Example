using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopifyAppKyle.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopifyShopDomain = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopifyShopId = table.Column<long>(nullable: false),
                    ShopifyShopDomain = table.Column<string>(nullable: true),
                    ShopifyAccessToken = table.Column<string>(nullable: true),
                    ShopifyChargeId = table.Column<long>(nullable: true),
                    BillingOn = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginStates");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
