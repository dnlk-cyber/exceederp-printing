namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory_archive : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.BinCardLineSecondary", "StoreItemAssignmentID", "EXCEED.StoreItemAssignment");
            DropIndex("EXCEED.BinCardLineSecondary", new[] { "StoreItemAssignmentID" });
            CreateTable(
                "Inventory_Archive.BinCardLineArchive",
                c => new
                    {
                        BinCardLineArchiveId = c.Int(nullable: false, identity: true),
                        BinCardLineID = c.Int(nullable: false),
                        StoreItemAssignmentID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReferenceDoc = c.String(),
                        ReceivedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedQTY = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IssuedUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Reference = c.String(),
                        TimeStamp = c.DateTime(),
                        IsVoid = c.Boolean(nullable: false),
                        SupplierName = c.String(),
                        IssuedTo = c.String(),
                        StoreCode = c.String(),
                        TransactionType = c.Int(nullable: false),
                        StatusType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BinCardLineArchiveId);
            
            CreateTable(
                "Inventory_Archive.StoreRequisitionArchive",
                c => new
                    {
                        StoreRequisitionArchiveId = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        Reference = c.String(),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        StoreCode = c.String(nullable: false),
                        ToStoreCode = c.String(),
                        IsFuel = c.Boolean(nullable: false),
                        IsTransfer = c.Boolean(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        IsFullfilled = c.Boolean(nullable: false),
                        IsForOther = c.Boolean(nullable: false),
                        ForBranchId = c.Int(),
                        ModelNo = c.String(),
                        JobOrderNo = c.String(),
                        PlateNo = c.String(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionArchiveId);
            
            CreateTable(
                "Inventory_Archive.StoreRequisitionItemArchive",
                c => new
                    {
                        StoreRequisitionItemArchiveId = c.Int(nullable: false, identity: true),
                        StoreRequisitionArchiveId = c.Int(nullable: false),
                        StoreRequisitionItemID = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        PartNo = c.String(),
                        ModelNo = c.String(),
                        SerialNo = c.String(),
                        ChasisNo = c.String(),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchaseQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        ItemSpecification = c.String(),
                        GeneratePurchaseReq = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionItemArchiveId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: false)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: false)
                .ForeignKey("Inventory_Archive.StoreRequisitionArchive", t => t.StoreRequisitionArchiveId, cascadeDelete: true)
                .Index(t => t.StoreRequisitionArchiveId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Inventory_Archive.StoreRequisitionPropertyValidationArchive",
                c => new
                    {
                        StoreRequisitionPropertyValidationArchiveId = c.Int(nullable: false, identity: true),
                        StoreRequisitionArchiveId = c.Int(nullable: false),
                        StoreRequisitionPropertyValidationID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        Approver = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionPropertyValidationArchiveId)
                .ForeignKey("Inventory_Archive.StoreRequisitionArchive", t => t.StoreRequisitionArchiveId, cascadeDelete: true)
                .Index(t => t.StoreRequisitionArchiveId);
            
            CreateTable(
                "Inventory_Archive.StoreRequisitionValidationArchive",
                c => new
                    {
                        StoreRequisitionValidationArchiveId = c.Int(nullable: false, identity: true),
                        StoreRequisitionArchiveId = c.Int(nullable: false),
                        StoreRequisitionValidationID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionValidationArchiveId)
                .ForeignKey("Inventory_Archive.StoreRequisitionArchive", t => t.StoreRequisitionArchiveId, cascadeDelete: true)
                .Index(t => t.StoreRequisitionArchiveId);
            
            DropTable("EXCEED.BinCardLineSecondary");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.BinCardLineSecondary",
                c => new
                    {
                        BinCardLineSecondaryId = c.Int(nullable: false, identity: true),
                        StoreItemAssignmentID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReferenceDoc = c.String(),
                        ReceivedQTY = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ReceivedUPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ReceivedTPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IssuedQTY = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IssuedUPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IssuedTPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        BalanceQTY = c.Decimal(nullable: false, precision: 18, scale: 4),
                        BalanceUPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        BalanceTPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Reference = c.String(),
                        TimeStamp = c.DateTime(),
                        IsVoid = c.Boolean(nullable: false),
                        SupplierName = c.String(),
                        IssuedTo = c.String(),
                        TransactionType = c.Int(nullable: false),
                        BinCardLineID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BinCardLineSecondaryId);
            
            DropForeignKey("Inventory_Archive.StoreRequisitionValidationArchive", "StoreRequisitionArchiveId", "Inventory_Archive.StoreRequisitionArchive");
            DropForeignKey("Inventory_Archive.StoreRequisitionPropertyValidationArchive", "StoreRequisitionArchiveId", "Inventory_Archive.StoreRequisitionArchive");
            DropForeignKey("Inventory_Archive.StoreRequisitionItemArchive", "StoreRequisitionArchiveId", "Inventory_Archive.StoreRequisitionArchive");
            DropForeignKey("Inventory_Archive.StoreRequisitionItemArchive", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Inventory_Archive.StoreRequisitionItemArchive", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("Inventory_Archive.StoreRequisitionValidationArchive", new[] { "StoreRequisitionArchiveId" });
            DropIndex("Inventory_Archive.StoreRequisitionPropertyValidationArchive", new[] { "StoreRequisitionArchiveId" });
            DropIndex("Inventory_Archive.StoreRequisitionItemArchive", new[] { "ItemCode" });
            DropIndex("Inventory_Archive.StoreRequisitionItemArchive", new[] { "ItemCategoryCode" });
            DropIndex("Inventory_Archive.StoreRequisitionItemArchive", new[] { "StoreRequisitionArchiveId" });
            DropTable("Inventory_Archive.StoreRequisitionValidationArchive");
            DropTable("Inventory_Archive.StoreRequisitionPropertyValidationArchive");
            DropTable("Inventory_Archive.StoreRequisitionItemArchive");
            DropTable("Inventory_Archive.StoreRequisitionArchive");
            DropTable("Inventory_Archive.BinCardLineArchive");
            CreateIndex("EXCEED.BinCardLineSecondary", "StoreItemAssignmentID");
            AddForeignKey("EXCEED.BinCardLineSecondary", "StoreItemAssignmentID", "EXCEED.StoreItemAssignment", "StoreItemAssignmentID", cascadeDelete: true);
        }
    }
}
