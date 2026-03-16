using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddShopOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopOwners",
                columns: table => new
                {
                    ShopOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    WalletBalance = table.Column<double>(type: "double precision", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOwners", x => x.ShopOwnerId);
                    table.ForeignKey(
                        name: "FK_ShopOwners_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "692accdb-33fa-46a2-8bae-604fe18a59bf", "AQAAAAIAAYagAAAAEJh4quLF82mWvCb4Ip7ee8r/gYIjR7O0WvVKf3QSxtvDres/NYAcmRUcVxD0zWd1rg==", "914225fc-bb92-4cb8-9ff5-b4fd06217faa" });

            migrationBuilder.CreateIndex(
                name: "IX_ShopOwners_UserId",
                table: "ShopOwners",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopOwners");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4798ea12-9851-4dc2-8c70-55c198e3310f", "AQAAAAIAAYagAAAAEGq1UAUQQuKSAFwPY32LkrzdqJRF/ZTeNKG+yq9IENHySUqPZ6wJtSiTS3D+kgj5OA==", "5edf25a3-0181-483d-aaf6-6ebdc1a0de46" });
        }
    }
}
