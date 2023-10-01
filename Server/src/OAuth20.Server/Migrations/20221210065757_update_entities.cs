using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth20.Server.Migrations
{
    public partial class update_entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                schema: "OAuth",
                table: "OAuthTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenType",
                schema: "OAuth",
                table: "OAuthTokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "OAuth",
                table: "OAuthTokens");

            migrationBuilder.DropColumn(
                name: "TokenType",
                schema: "OAuth",
                table: "OAuthTokens");
        }
    }
}
