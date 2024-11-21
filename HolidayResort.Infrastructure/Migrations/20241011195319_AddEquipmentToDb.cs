using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HolidayResort.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccommodationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_Accommodations_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "AccommodationId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, null, "Bračni krevet" },
                    { 2, 1, null, "Sef" },
                    { 3, 1, null, "Fen" },
                    { 4, 2, null, "Bračni krevet i sofa na razvlačenje" },
                    { 5, 2, null, "Minibar" },
                    { 6, 2, null, "Klima uređaj" },
                    { 7, 3, null, "Bračni krevet, sofa i kauč na razvlačenje" },
                    { 8, 3, null, "Perilica za pranje suđa" },
                    { 9, 3, null, "Kuhinja" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_AccommodationId",
                table: "Equipments",
                column: "AccommodationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipments");
        }
    }
}
