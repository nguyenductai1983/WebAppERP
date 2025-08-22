using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppERP.Migrations
{
    public partial class Add_phieukho1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierLotNumber",
                table: "PurchaseOrderDetails",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationDate",
                table: "MaterialIssues",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmedById",
                table: "MaterialIssues",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialIssues_ConfirmedById",
                table: "MaterialIssues",
                column: "ConfirmedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialIssues_AspNetUsers_ConfirmedById",
                table: "MaterialIssues",
                column: "ConfirmedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialIssues_AspNetUsers_ConfirmedById",
                table: "MaterialIssues");

            migrationBuilder.DropIndex(
                name: "IX_MaterialIssues_ConfirmedById",
                table: "MaterialIssues");

            migrationBuilder.DropColumn(
                name: "SupplierLotNumber",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "ConfirmationDate",
                table: "MaterialIssues");

            migrationBuilder.DropColumn(
                name: "ConfirmedById",
                table: "MaterialIssues");
        }
    }
}
