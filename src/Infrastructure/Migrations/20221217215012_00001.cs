using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uni.Scan.Infrastructure.Migrations
{
    public partial class _00001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AffectedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTask",
                columns: table => new
                {
                    ObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ResponsibleId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProcessingStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PriorityCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OperationType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OperationTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SiteName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTask", x => x.ObjectID);
                });

            migrationBuilder.CreateTable(
                name: "LabelTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ModelID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogisticArea",
                columns: table => new
                {
                    ObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LogisticAreaUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticAreaID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InventoryManagedLocationID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InventoryManagedLocationUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SiteID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SiteUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TypeCodeText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InventoryManagedLocationIndicator = table.Column<bool>(type: "bit", nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LifeCycleStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LifeCycleStatusCodeText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ParentLocationUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticArea", x => x.ObjectID);
                });

            migrationBuilder.CreateTable(
                name: "LogisticParametres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParametreID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ValueString = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ValueBin = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticParametres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogisticTask",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OperationTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProcessingStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ResponsibleId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ResponsibleName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TaskFolderId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PriorityCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ThirdPartyKey = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ThirdpartyName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    BusinessTransactionDocumentReferenceID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TaskUuid = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ReferencedObjectUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OperationActivityUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SiteName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProcessTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProcessType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ItemsNumberValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    ObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description_KUT = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductValuationLevelTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductValuationLevelTypeCodeText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SerialIdentifierAssignmentProfileCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SerialIdentifierAssignmentProfileCodeText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InternalID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductIdentifierTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductIdentifierTypeCodeText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductTypeCodeText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    BaseMeasureUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.ObjectID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScanningCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarCodeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BarCodePrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BarCodeSuffix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanningCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockAnomaly",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnomalyType = table.Column<int>(type: "int", nullable: false),
                    CompanyID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SiteID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OwnerPartyID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InventoryRestrictedUseIndicator = table.Column<bool>(type: "bit", nullable: false),
                    InventoryStockStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IdentifiedStockID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticsAreaID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    QuantityUniteCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IdentifiedStockType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IdentifiedStockTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticsArea = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CorrectedIdentifiedStockID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CorrectedQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    AnomalyReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AnomalyStatus = table.Column<int>(type: "int", nullable: false),
                    DeclaredBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CloseOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAnomaly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePictureDataUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SiteID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTaskOperation",
                columns: table => new
                {
                    ObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TaskObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ParentObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SiteID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticsAreaUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ApprovalProcessingStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CountLifeCycleStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticsAreaID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InventoryManagedLocationID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InventoryManagedLocationUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticsAreaTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticsAreaLifeCycleStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTaskOperation", x => x.ObjectID);
                    table.ForeignKey(
                        name: "FK_InventoryTask_Operation",
                        column: x => x.TaskObjectID,
                        principalTable: "InventoryTask",
                        principalColumn: "ObjectID");
                });

            migrationBuilder.CreateTable(
                name: "LogisticTaskDetail",
                columns: table => new
                {
                    OutputUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InputUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TaskId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LineItemID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SourceLogisticsAreaID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TargetLogisticsAreaID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PlanQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    PlanQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OpenQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    OpenQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ConfirmQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    ConfirmQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TotalConfirmedQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    TotalConfirmedQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IdentifiedStockID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PackageLogisticUnitUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PackageLogisticUnitTotalConfirmedQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    PackageLogisticUnitOpenQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    PackageLogisticUnitTotalConfirmedQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PackageLogisticUnitPlanQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MaterialDeviationStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MaterialInspectionID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PackageLogisticUnitPlanQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    PackageLogisticUnitOpenQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MaterialDeviationStatusCodeSpecified = table.Column<bool>(type: "bit", nullable: false),
                    LogisticsDeviationReasonCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LineItemIDSpecified = table.Column<bool>(type: "bit", nullable: false),
                    ProductSpecificationID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RestrictedIndicator = table.Column<bool>(type: "bit", nullable: false),
                    ProductRequirementSpecificationDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SerialNumberAssignments = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticTaskDetail", x => x.OutputUUID);
                    table.ForeignKey(
                        name: "FK_TaskTaskLine",
                        column: x => x.TaskId,
                        principalTable: "LogisticTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LogisticTaskLabel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LineItemID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PlanQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    IdentifiedStock = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SerialStock = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QuatityOnLabel = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    QuatityUnite = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExternalID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductSpecification = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GTIN = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PackageID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TransferOrdre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductionOrdre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FabricationOrdre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SupplierIdentifiedStock = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Tare = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    NbrEtiquettes = table.Column<int>(type: "int", nullable: false),
                    Duplicata = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticTaskLabel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLineLabel",
                        column: x => x.TaskId,
                        principalTable: "LogisticTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTaskItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OperationObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TaskID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogisticPackageUUID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IdentifiedStockID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CountedQuantityUnitCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CountCounterValue = table.Column<int>(type: "int", nullable: false),
                    DeviationReasonCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ForceSkippedIndicator = table.Column<bool>(type: "bit", nullable: false),
                    IncludeIndicator = table.Column<bool>(type: "bit", nullable: false),
                    InventoryItemNumberValue = table.Column<int>(type: "int", nullable: false),
                    ApprovalResultStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CountApprovalStatusCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ApprovalDiscrepancyPercent = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    CountedQuantityTypeCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ZeroCountedQuantityConfirmedIndicator = table.Column<bool>(type: "bit", nullable: false),
                    CountedQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    BookInventoryQuantity = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    QuantitObjectID = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTaskItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTaskOperation_Item",
                        column: x => x.OperationObjectID,
                        principalTable: "InventoryTaskOperation",
                        principalColumn: "ObjectID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTaskItem_OperationObjectID",
                table: "InventoryTaskItem",
                column: "OperationObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTaskOperation_TaskObjectID",
                table: "InventoryTaskOperation",
                column: "TaskObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticTaskDetail_TaskId",
                table: "LogisticTaskDetail",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticTaskLabel_TaskId",
                table: "LogisticTaskLabel",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "Identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Identity",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "Identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "Identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails");

            migrationBuilder.DropTable(
                name: "InventoryTaskItem");

            migrationBuilder.DropTable(
                name: "LabelTemplate");

            migrationBuilder.DropTable(
                name: "LogisticArea");

            migrationBuilder.DropTable(
                name: "LogisticParametres");

            migrationBuilder.DropTable(
                name: "LogisticTaskDetail");

            migrationBuilder.DropTable(
                name: "LogisticTaskLabel");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ScanningCode");

            migrationBuilder.DropTable(
                name: "StockAnomaly");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "InventoryTaskOperation");

            migrationBuilder.DropTable(
                name: "LogisticTask");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "InventoryTask");
        }
    }
}
