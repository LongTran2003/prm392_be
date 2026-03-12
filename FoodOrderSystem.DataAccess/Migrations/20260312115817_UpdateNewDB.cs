using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dc57164b-5ba7-4e9a-aeb9-d66348e1abec", "AQAAAAIAAYagAAAAEG5EwpcZojiXdj2Dvc/DITT6R9ZzKMfLhS1YuzF6Ad658zTSldCitJC2J12FBlId2A==", "7d28e30e-f350-4b8b-8fc3-d7cc41011b6f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3850ac9a-7596-4add-883e-7ebda5601d44", "AQAAAAIAAYagAAAAEKE5288JaRGE0lu7M+286pF3G89Jo8GE+nLgUoBJJNkfwvLJAk8iauuXV5CivApnIw==", "2f5c3140-3d5c-4949-be60-1ae6a5442761" });
        }
    }
}
