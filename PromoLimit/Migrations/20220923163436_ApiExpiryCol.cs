using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoLimit.Migrations
{
    public partial class ApiExpiryCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryTime",
                table: "MlInfos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryTime",
                table: "MlInfos");
        }
    }
}
