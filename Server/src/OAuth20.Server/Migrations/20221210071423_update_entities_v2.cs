using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth20.Server.Migrations
{
    public partial class update_entities_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "OAuth",
                table: "OAuthTokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "OAuth",
                table: "OAuthTokens");
        }
    }
}
