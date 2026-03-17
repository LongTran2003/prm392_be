using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedTimeToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "CreatedTime", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aed956dc-a300-456b-8df4-1b61a5040665", null, "AQAAAAIAAYagAAAAEOBr1nknaqy9XcN3cOFzquCmh2G+Q1u9qmCK4ViYmEWr5fhB8cYPucgMvjNWXAn5JQ==", "df80e3ca-b174-4449-9eb0-dcadec1d44fb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d66a01eb-f3ca-492a-851f-2cb6e196e25c", "AQAAAAIAAYagAAAAEGesrdVXVHZvp4CufTumsx/0YPKkheQtaM0gbLWjvmF9op11ZSZNFQeDntARr1LOUg==", "a5f72077-c81e-4b7b-bea0-33b72036c08e" });
        }
    }
}
