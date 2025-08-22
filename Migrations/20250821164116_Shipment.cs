using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppERP.Migrations
{
    public partial class Shipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ShippedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ShippedById = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shipments_AspNetUsers_ShippedById",
                        column: x => x.ShippedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    QuantityToShip = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_ProductId",
                table: "ShipmentDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_ShipmentId",
                table: "ShipmentDetails",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_SalesOrderId",
                table: "Shipments",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShippedById",
                table: "Shipments",
                column: "ShippedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentDetails");

            migrationBuilder.DropTable(
                name: "Shipments");
        }
    }
}
