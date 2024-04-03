using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deviceflow_ConsoleApp.Migrations
{
    public partial class deviceflowclienttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceFlowClients",
                columns: table => new
                {
                    DeviceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceFlowClients", x => x.DeviceCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceFlowClients_DeviceCode",
                table: "DeviceFlowClients",
                column: "DeviceCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceFlowClients");
        }
    }
}
