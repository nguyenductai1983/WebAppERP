using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppERP.Migrations
{
    public partial class FinalizeDbContextConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialConsumptionLogs_ProductionLogs_ProductionLogId",
                table: "MaterialConsumptionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialConsumptionLogs_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialConsumptionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialIssueDetails_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialIssueDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId",
                table: "WorkOrderBOMs");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId1",
                table: "WorkOrderBOMs");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderBOMs_WorkOrderId1",
                table: "WorkOrderBOMs");

            migrationBuilder.DropColumn(
                name: "WorkOrderId1",
                table: "WorkOrderBOMs");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityIssued",
                table: "MaterialIssueDetails",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "MaterialRequisitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(nullable: false),
                    WorkshopId = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    RequestedById = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRequisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialRequisitions_AspNetUsers_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialRequisitions_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialRequisitions_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialRequisitionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialRequisitionId = table.Column<int>(nullable: false),
                    WorkOrderBOMId = table.Column<int>(nullable: false),
                    QuantityRequested = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRequisitionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialRequisitionDetails_MaterialRequisitions_MaterialRequisitionId",
                        column: x => x.MaterialRequisitionId,
                        principalTable: "MaterialRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialRequisitionDetails_WorkOrderBOMs_WorkOrderBOMId",
                        column: x => x.WorkOrderBOMId,
                        principalTable: "WorkOrderBOMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequisitionDetails_MaterialRequisitionId",
                table: "MaterialRequisitionDetails",
                column: "MaterialRequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequisitionDetails_WorkOrderBOMId",
                table: "MaterialRequisitionDetails",
                column: "WorkOrderBOMId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequisitions_RequestedById",
                table: "MaterialRequisitions",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequisitions_WorkOrderId",
                table: "MaterialRequisitions",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequisitions_WorkshopId",
                table: "MaterialRequisitions",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialConsumptionLogs_ProductionLogs_ProductionLogId",
                table: "MaterialConsumptionLogs",
                column: "ProductionLogId",
                principalTable: "ProductionLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialConsumptionLogs_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialConsumptionLogs",
                column: "WorkOrderBOMId",
                principalTable: "WorkOrderBOMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialIssueDetails_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialIssueDetails",
                column: "WorkOrderBOMId",
                principalTable: "WorkOrderBOMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId",
                table: "WorkOrderBOMs",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialConsumptionLogs_ProductionLogs_ProductionLogId",
                table: "MaterialConsumptionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialConsumptionLogs_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialConsumptionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialIssueDetails_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialIssueDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId",
                table: "WorkOrderBOMs");

            migrationBuilder.DropTable(
                name: "MaterialRequisitionDetails");

            migrationBuilder.DropTable(
                name: "MaterialRequisitions");

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId1",
                table: "WorkOrderBOMs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityIssued",
                table: "MaterialIssueDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderBOMs_WorkOrderId1",
                table: "WorkOrderBOMs",
                column: "WorkOrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialConsumptionLogs_ProductionLogs_ProductionLogId",
                table: "MaterialConsumptionLogs",
                column: "ProductionLogId",
                principalTable: "ProductionLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialConsumptionLogs_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialConsumptionLogs",
                column: "WorkOrderBOMId",
                principalTable: "WorkOrderBOMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialIssueDetails_WorkOrderBOMs_WorkOrderBOMId",
                table: "MaterialIssueDetails",
                column: "WorkOrderBOMId",
                principalTable: "WorkOrderBOMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId",
                table: "WorkOrderBOMs",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId1",
                table: "WorkOrderBOMs",
                column: "WorkOrderId1",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
