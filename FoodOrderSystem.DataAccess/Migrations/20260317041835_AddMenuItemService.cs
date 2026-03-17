using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItemService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f68307f0-03a3-4e5d-a02c-cb31d8a6e6b3", "AQAAAAIAAYagAAAAEH5mbA1liG+EN4pL5gx/bvLqB9tAygf1QB1ChAJOFrkBR1K7uXwJFlnH/DzA3qkLgw==", "327937fb-9345-446d-b166-17cf1d746b31" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf3f9129-422e-46db-80c3-099df77c53d3", "AQAAAAIAAYagAAAAEARAYZkfymD/vpO4XdQn9REHwO/sdml7z8daV9OkUl5RqvQFeeVNTzLZsZ0JNgc4tQ==", "04958dc2-bd99-4793-9af6-ea1bdda34a57" });
        }
    }
}
