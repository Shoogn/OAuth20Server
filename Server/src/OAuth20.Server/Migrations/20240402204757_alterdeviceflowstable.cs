using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth20.Server.Migrations
{
    public partial class alterdeviceflowstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestedScope",
                schema: "OAuth",
                table: "DeviceFlows",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedScope",
                schema: "OAuth",
                table: "DeviceFlows");
        }
    }
}
