using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnicalTest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "COM_CUSTOMER",
                columns: table => new
                {
                    ComCustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COM_CUSTOMER", x => x.ComCustomerId);
                });

            migrationBuilder.CreateTable(
                name: "SO_ORDER",
                columns: table => new
                {
                    SoOrderId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComCustomerId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SO_ORDER", x => x.SoOrderId);
                    table.ForeignKey(
                        name: "FK_SO_ORDER_COM_CUSTOMER_ComCustomerId",
                        column: x => x.ComCustomerId,
                        principalTable: "COM_CUSTOMER",
                        principalColumn: "ComCustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SO_ITEM",
                columns: table => new
                {
                    SoItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoOrderId = table.Column<long>(type: "bigint", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SO_ITEM", x => x.SoItemId);
                    table.ForeignKey(
                        name: "FK_SO_ITEM_SO_ORDER_SoOrderId",
                        column: x => x.SoOrderId,
                        principalTable: "SO_ORDER",
                        principalColumn: "SoOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SO_ITEM_SoOrderId",
                table: "SO_ITEM",
                column: "SoOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SO_ORDER_ComCustomerId",
                table: "SO_ORDER",
                column: "ComCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SO_ITEM");

            migrationBuilder.DropTable(
                name: "SO_ORDER");

            migrationBuilder.DropTable(
                name: "COM_CUSTOMER");
        }
    }
}
