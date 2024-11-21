using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HolidayResort.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccommodationToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accommodations",
                columns: new[] { "Id", "Capacity", "CreatedDate", "Description", "ImageUrl", "Name", "Price", "SquareMeter", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moguće je rezervirati noćenje s doručkom, polupansion ili puni pansion.", "https://placehold.co/600x400", "Soba", 60.0, 40, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moguće je rezervirati noćenje s doručkom, polupansion ili puni pansion.", "https://placehold.co/600x400", "Bungalov", 80.0, 50, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moguće je rezervirati noćenje s doručkom, polupansion ili puni pansion.", "https://placehold.co/600x400", "Apartman", 100.0, 60, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accommodations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accommodations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accommodations",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
