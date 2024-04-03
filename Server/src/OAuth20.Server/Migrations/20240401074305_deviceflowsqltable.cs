using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth20.Server.Migrations
{
    public partial class deviceflowsqltable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceFlows",
                schema: "OAuth",
                columns: table => new
                {
                    UserCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpireIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserInterActionComplete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceFlows", x => x.UserCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceFlows_UserCode",
                schema: "OAuth",
                table: "DeviceFlows",
                column: "UserCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceFlows",
                schema: "OAuth");
        }
    }
}
