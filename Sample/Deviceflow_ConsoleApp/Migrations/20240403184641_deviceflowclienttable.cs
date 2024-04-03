using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deviceflow_ConsoleApp.Migrations
{
    public partial class deviceflowclienttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "oauthclient");

            migrationBuilder.CreateTable(
                name: "DeviceFlowClients",
                schema: "oauthclient",
                columns: table => new
                {
                    DeviceCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceFlowClients", x => x.DeviceCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceFlowClients_DeviceCode",
                schema: "oauthclient",
                table: "DeviceFlowClients",
                column: "DeviceCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceFlowClients",
                schema: "oauthclient");
        }
    }
}
