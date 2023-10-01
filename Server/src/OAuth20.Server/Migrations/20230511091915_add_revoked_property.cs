using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth20.Server.Migrations
{
    public partial class add_revoked_property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Revoked",
                schema: "OAuth",
                table: "OAuthTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revoked",
                schema: "OAuth",
                table: "OAuthTokens");
        }
    }
}
