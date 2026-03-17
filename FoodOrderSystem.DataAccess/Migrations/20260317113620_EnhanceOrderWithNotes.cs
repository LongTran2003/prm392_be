using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceOrderWithNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedToFirebase",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d66a01eb-f3ca-492a-851f-2cb6e196e25c", "AQAAAAIAAYagAAAAEGesrdVXVHZvp4CufTumsx/0YPKkheQtaM0gbLWjvmF9op11ZSZNFQeDntARr1LOUg==", "a5f72077-c81e-4b7b-bea0-33b72036c08e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSyncedToFirebase",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84da5dcb-cb4d-44da-91eb-78b9af62d8cc", "AQAAAAIAAYagAAAAEFg+5KsNbtGAhnNCwU7dxzRr+5g4IQoJ6l/eA+C9xLl9qAeTxXz8/vCVSEAV8EVvLA==", "99e8bbdd-2243-44c4-b87a-14a0b3f3beb8" });
        }
    }
}
