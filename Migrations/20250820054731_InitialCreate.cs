using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppERP.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(maxLength: 150, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextileType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextileType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Workshops",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarnType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarnType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(maxLength: 100, nullable: false),
                    Position = table.Column<string>(maxLength: 50, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Address = table.Column<string>(maxLength: 250, nullable: false),
                    CitizenId = table.Column<string>(maxLength: 12, nullable: false),
                    HireDate = table.Column<DateTime>(nullable: false),
                    Department = table.Column<string>(maxLength: 100, nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    ShippingAddress = table.Column<string>(maxLength: 250, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    DebitOrCredit = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(nullable: true),
                    SupplierId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionStages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    WorkshopId = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    ProcessingMethod = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionStages_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    RequestingEmployeeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Employees_RequestingEmployeeId",
                        column: x => x.RequestingEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Method = table.Column<int>(nullable: false),
                    Note = table.Column<string>(maxLength: 500, nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true),
                    PurchaseOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payment_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MachineType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ProductionStageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineType", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MachineType_ProductionStages_ProductionStageId",
                        column: x => x.ProductionStageId,
                        principalTable: "ProductionStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Sku = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StandardLaborCost = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StandardOverheadCost = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    UnitOfMeasure = table.Column<string>(maxLength: 50, nullable: true),
                    Color = table.Column<string>(maxLength: 50, nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    Gsm = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    LotEntityName = table.Column<string>(maxLength: 50, nullable: true),
                    DefaultProductionStageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductionStages_DefaultProductionStageId",
                        column: x => x.DefaultProductionStageId,
                        principalTable: "ProductionStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Machine",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    WorkshopId = table.Column<int>(nullable: false),
                    MachineTypeId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machine", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Machine_MachineType_MachineTypeId",
                        column: x => x.MachineTypeId,
                        principalTable: "MachineType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Machine_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillOfMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinishedProductId = table.Column<int>(nullable: false),
                    ProductionStageId = table.Column<int>(nullable: false),
                    ComponentId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    PieceLength = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    PieceWidth = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    PiecesPerProduct = table.Column<int>(nullable: true),
                    Gsm = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Notes = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillOfMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillOfMaterials_Products_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillOfMaterials_Products_FinishedProductId",
                        column: x => x.FinishedProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillOfMaterials_ProductionStages_ProductionStageId",
                        column: x => x.ProductionStageId,
                        principalTable: "ProductionStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Color = table.Column<string>(maxLength: 50, nullable: true),
                    GsmOrGlm = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    HasLamination = table.Column<bool>(nullable: false),
                    FabricWidth = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    Meterage = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    QuantityPerBag = table.Column<int>(nullable: false),
                    Specification = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderDetails_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    ProductionStageId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    QuantityToProduce = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ActualMaterialCost = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ActualLaborCost = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ActualOverheadCost = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ParentWorkOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_WorkOrders_ParentWorkOrderId",
                        column: x => x.ParentWorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_ProductionStages_ProductionStageId",
                        column: x => x.ProductionStageId,
                        principalTable: "ProductionStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoatedTextile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(nullable: false),
                    OperatorId = table.Column<string>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    InitialLength = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    Quality = table.Column<string>(maxLength: 50, nullable: true),
                    Created_at = table.Column<DateTime>(nullable: false),
                    Updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoatedTextile", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CoatedTextile_Machine_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machine",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoatedTextile_AspNetUsers_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanDate = table.Column<DateTime>(nullable: false),
                    SalesOrderId = table.Column<int>(nullable: true),
                    WorkOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionPlans_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionPlans_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderBOMs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(nullable: false),
                    ComponentId = table.Column<int>(nullable: false),
                    RequiredQuantity = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ConsumedQuantity = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    WorkOrderId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderBOMs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrderBOMs_Products_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrderBOMs_WorkOrders_WorkOrderId1",
                        column: x => x.WorkOrderId1,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderRoutings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(nullable: false),
                    ProductionStageId = table.Column<int>(nullable: false),
                    QuantityToProduce = table.Column<int>(nullable: false),
                    QuantityProduced = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderRoutings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrderRoutings_ProductionStages_ProductionStageId",
                        column: x => x.ProductionStageId,
                        principalTable: "ProductionStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrderRoutings_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderRoutingId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    LogDate = table.Column<DateTime>(nullable: false),
                    OperatorId = table.Column<string>(nullable: false),
                    MachineId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(maxLength: 500, nullable: true),
                    AttributesJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionLogs_Machine_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machine",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionLogs_AspNetUsers_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionLogs_WorkOrderRoutings_WorkOrderRoutingId",
                        column: x => x.WorkOrderRoutingId,
                        principalTable: "WorkOrderRoutings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialConsumptionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionLogId = table.Column<int>(nullable: false),
                    WorkOrderBOMId = table.Column<int>(nullable: false),
                    ConsumedLotId = table.Column<int>(nullable: true),
                    QuantityConsumed = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialConsumptionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialConsumptionLogs_ProductionLogs_ProductionLogId",
                        column: x => x.ProductionLogId,
                        principalTable: "ProductionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialConsumptionLogs_WorkOrderBOMs_WorkOrderBOMId",
                        column: x => x.WorkOrderBOMId,
                        principalTable: "WorkOrderBOMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Yarn",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    OperatorId = table.Column<string>(nullable: false),
                    MachineId = table.Column<int>(nullable: false),
                    SpoolCount = table.Column<int>(nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    SpoolWeight = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    StockQuantity = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    WorkOrderId = table.Column<int>(nullable: true),
                    Created_at = table.Column<DateTime>(nullable: true),
                    Updated_at = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProductionLogId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yarn", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Yarn_Machine_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machine",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Yarn_AspNetUsers_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yarn_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Yarn_ProductionLogs_ProductionLogId",
                        column: x => x.ProductionLogId,
                        principalTable: "ProductionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yarn_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Textile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextileTypeId = table.Column<int>(nullable: false),
                    MachineId = table.Column<int>(nullable: false),
                    OperatorId = table.Column<string>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    ActualWidth = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    InitialLength = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    Quality = table.Column<bool>(nullable: false),
                    SourceYarnId = table.Column<int>(nullable: true),
                    ProductionLogId = table.Column<int>(nullable: true),
                    WorkOrderId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Created_at = table.Column<DateTime>(nullable: true),
                    Updated_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Textile", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Textile_Machine_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machine",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Textile_AspNetUsers_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Textile_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Textile_ProductionLogs_ProductionLogId",
                        column: x => x.ProductionLogId,
                        principalTable: "ProductionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Textile_Yarn_SourceYarnId",
                        column: x => x.SourceYarnId,
                        principalTable: "Yarn",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Textile_TextileType_TextileTypeId",
                        column: x => x.TextileTypeId,
                        principalTable: "TextileType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Textile_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TextileYarnUsage",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextileId = table.Column<int>(nullable: false),
                    YarnId = table.Column<int>(nullable: false),
                    QuantityUsed = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    UsageDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextileYarnUsage", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TextileYarnUsage_Textile_TextileId",
                        column: x => x.TextileId,
                        principalTable: "Textile",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TextileYarnUsage_Yarn_YarnId",
                        column: x => x.YarnId,
                        principalTable: "Yarn",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BillOfMaterials_ComponentId",
                table: "BillOfMaterials",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_BillOfMaterials_FinishedProductId",
                table: "BillOfMaterials",
                column: "FinishedProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BillOfMaterials_ProductionStageId",
                table: "BillOfMaterials",
                column: "ProductionStageId");

            migrationBuilder.CreateIndex(
                name: "IX_CoatedTextile_MachineId",
                table: "CoatedTextile",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_CoatedTextile_OperatorId",
                table: "CoatedTextile",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountId",
                table: "JournalEntryLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_JournalEntryId",
                table: "JournalEntryLines",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_RequestingEmployeeId",
                table: "LeaveRequests",
                column: "RequestingEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Machine_MachineTypeId",
                table: "Machine",
                column: "MachineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Machine_WorkshopId",
                table: "Machine",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineType_ProductionStageId",
                table: "MachineType",
                column: "ProductionStageId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialConsumptionLogs_ProductionLogId",
                table: "MaterialConsumptionLogs",
                column: "ProductionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialConsumptionLogs_WorkOrderBOMId",
                table: "MaterialConsumptionLogs",
                column: "WorkOrderBOMId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PurchaseOrderId",
                table: "Payment",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_SalesOrderId",
                table: "Payment",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLogs_MachineId",
                table: "ProductionLogs",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLogs_OperatorId",
                table: "ProductionLogs",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLogs_WorkOrderRoutingId",
                table: "ProductionLogs",
                column: "WorkOrderRoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionPlans_SalesOrderId",
                table: "ProductionPlans",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionPlans_WorkOrderId",
                table: "ProductionPlans",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionStages_WorkshopId",
                table: "ProductionStages",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DefaultProductionStageId",
                table: "Products",
                column: "DefaultProductionStageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_ProductId",
                table: "PurchaseOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_PurchaseOrderId",
                table: "PurchaseOrderDetails",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderDetails_ProductId",
                table: "SalesOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderDetails_SalesOrderId",
                table: "SalesOrderDetails",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Textile_MachineId",
                table: "Textile",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Textile_OperatorId",
                table: "Textile",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Textile_ProductId",
                table: "Textile",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Textile_ProductionLogId",
                table: "Textile",
                column: "ProductionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Textile_SourceYarnId",
                table: "Textile",
                column: "SourceYarnId");

            migrationBuilder.CreateIndex(
                name: "IX_Textile_TextileTypeId",
                table: "Textile",
                column: "TextileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Textile_WorkOrderId",
                table: "Textile",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TextileYarnUsage_TextileId",
                table: "TextileYarnUsage",
                column: "TextileId");

            migrationBuilder.CreateIndex(
                name: "IX_TextileYarnUsage_YarnId",
                table: "TextileYarnUsage",
                column: "YarnId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderBOMs_ComponentId",
                table: "WorkOrderBOMs",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderBOMs_WorkOrderId",
                table: "WorkOrderBOMs",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderBOMs_WorkOrderId1",
                table: "WorkOrderBOMs",
                column: "WorkOrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRoutings_ProductionStageId",
                table: "WorkOrderRoutings",
                column: "ProductionStageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRoutings_WorkOrderId",
                table: "WorkOrderRoutings",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ParentWorkOrderId",
                table: "WorkOrders",
                column: "ParentWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ProductId",
                table: "WorkOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ProductionStageId",
                table: "WorkOrders",
                column: "ProductionStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Yarn_MachineId",
                table: "Yarn",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Yarn_OperatorId",
                table: "Yarn",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Yarn_ProductId",
                table: "Yarn",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Yarn_ProductionLogId",
                table: "Yarn",
                column: "ProductionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Yarn_WorkOrderId",
                table: "Yarn",
                column: "WorkOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BillOfMaterials");

            migrationBuilder.DropTable(
                name: "CoatedTextile");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "JournalEntryLines");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "MaterialConsumptionLogs");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "ProductionPlans");

            migrationBuilder.DropTable(
                name: "PurchaseOrderDetails");

            migrationBuilder.DropTable(
                name: "SalesOrderDetails");

            migrationBuilder.DropTable(
                name: "TextileYarnUsage");

            migrationBuilder.DropTable(
                name: "YarnType");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "WorkOrderBOMs");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "SalesOrders");

            migrationBuilder.DropTable(
                name: "Textile");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Yarn");

            migrationBuilder.DropTable(
                name: "TextileType");

            migrationBuilder.DropTable(
                name: "ProductionLogs");

            migrationBuilder.DropTable(
                name: "Machine");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "WorkOrderRoutings");

            migrationBuilder.DropTable(
                name: "MachineType");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductionStages");

            migrationBuilder.DropTable(
                name: "Workshops");
        }
    }
}
