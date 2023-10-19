using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.DbContextMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "canteen",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    city = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    servesHotMeals = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_canteen", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exampleProductLists",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exampleProductLists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    securityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    studentNumber = table.Column<int>(type: "int", nullable: false),
                    studyCity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "canteenStaffMembers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    securityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    staffNumber = table.Column<int>(type: "int", nullable: false),
                    Canteenid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_canteenStaffMembers", x => x.id);
                    table.ForeignKey(
                        name: "FK_canteenStaffMembers_canteen_Canteenid",
                        column: x => x.Canteenid,
                        principalTable: "canteen",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    alcoholic = table.Column<bool>(type: "bit", nullable: false),
                    imageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExampleProductListid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_exampleProductLists_ExampleProductListid",
                        column: x => x.ExampleProductListid,
                        principalTable: "exampleProductLists",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "packets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    city = table.Column<int>(type: "int", nullable: false),
                    startPickup = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endPickup = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reservedByid = table.Column<int>(type: "int", nullable: true),
                    typeOfMeal = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    eighteenUp = table.Column<bool>(type: "bit", nullable: false),
                    Canteenid = table.Column<int>(type: "int", nullable: true),
                    exampleProductListid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_packets", x => x.id);
                    table.ForeignKey(
                        name: "FK_packets_canteen_Canteenid",
                        column: x => x.Canteenid,
                        principalTable: "canteen",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_packets_exampleProductLists_exampleProductListid",
                        column: x => x.exampleProductListid,
                        principalTable: "exampleProductLists",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_packets_students_reservedByid",
                        column: x => x.reservedByid,
                        principalTable: "students",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_canteenStaffMembers_Canteenid",
                table: "canteenStaffMembers",
                column: "Canteenid");

            migrationBuilder.CreateIndex(
                name: "IX_packets_Canteenid",
                table: "packets",
                column: "Canteenid");

            migrationBuilder.CreateIndex(
                name: "IX_packets_exampleProductListid",
                table: "packets",
                column: "exampleProductListid");

            migrationBuilder.CreateIndex(
                name: "IX_packets_reservedByid",
                table: "packets",
                column: "reservedByid");

            migrationBuilder.CreateIndex(
                name: "IX_products_ExampleProductListid",
                table: "products",
                column: "ExampleProductListid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "canteenStaffMembers");

            migrationBuilder.DropTable(
                name: "packets");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "canteen");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "exampleProductLists");
        }
    }
}
