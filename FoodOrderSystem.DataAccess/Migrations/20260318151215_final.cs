using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalRatings",
                table: "Shops",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShopId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<string>(type: "text", nullable: false),
                    StarRating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Ratings_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatingImages",
                columns: table => new
                {
                    RatingImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RatingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingImages", x => x.RatingImageId);
                    table.ForeignKey(
                        name: "FK_RatingImages_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "RatingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ecd75ab3-adbf-4a52-8daf-0b6c46647eda", "AQAAAAIAAYagAAAAEFUr/UvVzvedhcm3XShbXG9a+MA2Zp1hLPtYNqDAzqG8fYLbPopj74XBKD9KwTAyuw==", "4deb0aaf-4bb5-4f02-a881-2c5a2eb38491" });

            migrationBuilder.CreateIndex(
                name: "IX_RatingImages_RatingId",
                table: "RatingImages",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_CustomerId",
                table: "Ratings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ShopId",
                table: "Ratings",
                column: "ShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatingImages");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropColumn(
                name: "TotalRatings",
                table: "Shops");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "FoodOrder-Admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aed956dc-a300-456b-8df4-1b61a5040665", "AQAAAAIAAYagAAAAEOBr1nknaqy9XcN3cOFzquCmh2G+Q1u9qmCK4ViYmEWr5fhB8cYPucgMvjNWXAn5JQ==", "df80e3ca-b174-4449-9eb0-dcadec1d44fb" });
        }
    }
}
