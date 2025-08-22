using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppERP.Migrations
{
    public partial class MaterialIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialIssues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    IssuedDate = table.Column<DateTime>(nullable: true),
                    IssuedById = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialIssues_AspNetUsers_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialIssues_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialIssueDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialIssueId = table.Column<int>(nullable: false),
                    WorkOrderBOMId = table.Column<int>(nullable: false),
                    LotId = table.Column<int>(nullable: true),
                    QuantityIssued = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialIssueDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialIssueDetails_MaterialIssues_MaterialIssueId",
                        column: x => x.MaterialIssueId,
                        principalTable: "MaterialIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialIssueDetails_WorkOrderBOMs_WorkOrderBOMId",
                        column: x => x.WorkOrderBOMId,
                        principalTable: "WorkOrderBOMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssueDetails_MaterialIssueId",
                table: "MaterialIssueDetails",
                column: "MaterialIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssueDetails_WorkOrderBOMId",
                table: "MaterialIssueDetails",
                column: "WorkOrderBOMId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssues_IssuedById",
                table: "MaterialIssues",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssues_WorkOrderId",
                table: "MaterialIssues",
                column: "WorkOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialIssueDetails");

            migrationBuilder.DropTable(
                name: "MaterialIssues");
        }
    }
}
