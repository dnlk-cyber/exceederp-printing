namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventofixedprocfin : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.BidParticipatingSupplierItem", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetCount", "BranchId", "HRM.Branch");
            DropForeignKey("EXCEED.FixedAssetCount", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.InventoryItem", "UoMName", "EXCEED.UoM");
            DropIndex("EXCEED.InventoryItem", new[] { "UoMName" });
            DropIndex("EXCEED.BidParticipatingSupplierItem", new[] { "LotID" });
            DropIndex("EXCEED.Disposal", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetCount", new[] { "BranchId" });
            DropIndex("EXCEED.FixedAssetCount", new[] { "StoreCode" });
            CreateTable(
                "EXCEED.PaymentRequestAuthorizeValidation",
                c => new
                    {
                        PaymentRequestAuthorizeValidationId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PaymentRequestAuthorizeValidationId)
                .ForeignKey("EXCEED.PaymentRequest", t => t.PaymentRequestId, cascadeDelete: true)
                .Index(t => t.PaymentRequestId);
            
            CreateTable(
                "EXCEED.StoreRequisitionAttachment",
                c => new
                    {
                        StoreRequisitionAttachmentId = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionAttachmentId)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            AddColumn("EXCEED.AssetRegistration", "BusinessUnitId", c => c.Int());
            AddColumn("EXCEED.AssetRegistration", "FixedAssetNameID", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetRegistration", "SIVDate", c => c.DateTime());
            AddColumn("EXCEED.AssetRegistration", "Color", c => c.String());
            AddColumn("EXCEED.AssetDisposal", "IsExternalCustomer", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetDisposal", "ExternalCustomer", c => c.String());
            AddColumn("EXCEED.LocationTracking", "DisposalType", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentMethod", "BusinessUnitId", c => c.Int());
            AddColumn("EXCEED.StoreItemAssignment", "ShelfNo", c => c.String());
            AddColumn("EXCEED.StoreItemAssignment", "Row", c => c.String());
            AddColumn("EXCEED.StoreItemAssignment", "Column", c => c.String());
            AddColumn("EXCEED.BinCardLine", "TransactionOrder", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualNeedAssesment", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "ConfirmReceive", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetTransferReceive", "AssetTransferConfirm", c => c.Boolean(nullable: false));
            AddColumn("Inventory_Archive.BinCardLineArchive", "TransactionOrder", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetCountItem", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FixedAssetCountItem", "AssetStatus", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetCountItem", "EmployeeId", c => c.Int());
            AddColumn("EXCEED.FixedAssetCountItem", "StoreCode", c => c.String());
            AddColumn("EXCEED.FixedAssetCount", "BusinessUnitId", c => c.Int());
            AddColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetNameID", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisitionItem", "FixedAssetNameID", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "IsAuthorized", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "AuthorizedBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "AuthorizedTime", c => c.DateTime());
            AddColumn("EXCEED.GoodReceive", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GoodSellingAndLending", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GoodSellingAndLending", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Guarantee", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Guarantee", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Issue", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ItemTransferReceive", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProcurementPlan", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PropertyCount", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchasePayment", "ServiceCharge", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchasePayment", "OtherPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchaseReturn", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseReturn", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.StockAdjustment", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.StockAdjustment", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "IsForMaintenance", c => c.Boolean(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "IsExternalDriver", c => c.Boolean(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "ExternalDriver", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "IsForMaintenance", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "IsExternalDriver", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "ExternalDriver", c => c.String());
            AddColumn("EXCEED.StoreReturn", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.StoreReturn", "BusinessUnitId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.AssetDisposal", "CustomerId", c => c.String());
            AlterColumn("EXCEED.LocationTracking", "TransactionType", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PaymentMethod", "BranchId", c => c.Int());
            AlterColumn("EXCEED.InventoryItem", "UoMName", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.FixedAssetReceiveItem", "Name", c => c.String());
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "UoM", c => c.String(nullable: false));
            AlterColumn("EXCEED.ProcurementSupplier", "TIN", c => c.String(nullable: false));
            AlterColumn("EXCEED.StockAdjustmentItems", "StockAdjustmentType", c => c.Int(nullable: false));
            CreateIndex("EXCEED.InventoryItem", "UoMName");
            AddForeignKey("EXCEED.InventoryItem", "UoMName", "EXCEED.UoM", "UoMName", cascadeDelete: true);
            DropColumn("EXCEED.BidParticipatingSupplierItem", "SupplierId");
            DropColumn("EXCEED.BidParticipatingSupplierItem", "IsNewSupplier");
            DropColumn("EXCEED.BidParticipatingSupplierItem", "SupplierName");
            DropColumn("EXCEED.BidParticipatingSupplierItem", "LotID");
            DropColumn("EXCEED.FixedAssetCountItem", "Price");
            DropColumn("EXCEED.FixedAssetCountItem", "Location");
            DropColumn("EXCEED.FixedAssetCountItem", "ItemName");
            DropColumn("EXCEED.FixedAssetCount", "StoreCode");
            DropColumn("EXCEED.FixedAssetCount", "IsMidOfPeriod");
            DropColumn("EXCEED.PaymentRequest", "Type");
            DropColumn("EXCEED.IssueItem", "MaintenanceJobNo");
            DropColumn("EXCEED.StoreRequisitionItem", "ProductionJobNo");
            DropColumn("EXCEED.StoreRequisitionItem", "MaintenanceJobNo");
            DropColumn("EXCEED.StoreReturnItems", "MaintenanceJobNo");
            DropTable("EXCEED.Disposal");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.Disposal",
                c => new
                    {
                        DisposalId = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
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
                .PrimaryKey(t => t.DisposalId);
            
            AddColumn("EXCEED.StoreReturnItems", "MaintenanceJobNo", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "MaintenanceJobNo", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "ProductionJobNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "MaintenanceJobNo", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "Type", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetCount", "IsMidOfPeriod", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetCount", "StoreCode", c => c.String(maxLength: 128));
            AddColumn("EXCEED.FixedAssetCountItem", "ItemName", c => c.String());
            AddColumn("EXCEED.FixedAssetCountItem", "Location", c => c.String());
            AddColumn("EXCEED.FixedAssetCountItem", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidParticipatingSupplierItem", "LotID", c => c.Int(nullable: false));
            AddColumn("EXCEED.BidParticipatingSupplierItem", "SupplierName", c => c.String());
            AddColumn("EXCEED.BidParticipatingSupplierItem", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BidParticipatingSupplierItem", "SupplierId", c => c.String());
            DropForeignKey("EXCEED.InventoryItem", "UoMName", "EXCEED.UoM");
            DropForeignKey("EXCEED.StoreRequisitionAttachment", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.PaymentRequestAuthorizeValidation", "PaymentRequestId", "EXCEED.PaymentRequest");
            DropIndex("EXCEED.StoreRequisitionAttachment", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.PaymentRequestAuthorizeValidation", new[] { "PaymentRequestId" });
            DropIndex("EXCEED.InventoryItem", new[] { "UoMName" });
            AlterColumn("EXCEED.StockAdjustmentItems", "StockAdjustmentType", c => c.String());
            AlterColumn("EXCEED.ProcurementSupplier", "TIN", c => c.String());
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "UoM", c => c.String());
            AlterColumn("EXCEED.FixedAssetReceiveItem", "Name", c => c.String(nullable: false));
            AlterColumn("EXCEED.InventoryItem", "UoMName", c => c.String(maxLength: 128));
            AlterColumn("EXCEED.PaymentMethod", "BranchId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.LocationTracking", "TransactionType", c => c.String());
            AlterColumn("EXCEED.AssetDisposal", "CustomerId", c => c.String(nullable: false));
            DropColumn("EXCEED.StoreReturn", "BusinessUnitId");
            DropColumn("EXCEED.StoreReturn", "BranchId");
            DropColumn("EXCEED.StoreRequisition", "ExternalDriver");
            DropColumn("EXCEED.StoreRequisition", "IsExternalDriver");
            DropColumn("EXCEED.StoreRequisition", "IsForMaintenance");
            DropColumn("EXCEED.StoreRequisition", "BusinessUnitId");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "ExternalDriver");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "IsExternalDriver");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "IsForMaintenance");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "BusinessUnitId");
            DropColumn("EXCEED.StockAdjustment", "BusinessUnitId");
            DropColumn("EXCEED.StockAdjustment", "BranchId");
            DropColumn("EXCEED.PurchaseReturn", "BusinessUnitId");
            DropColumn("EXCEED.PurchaseReturn", "BranchId");
            DropColumn("EXCEED.PurchasePayment", "OtherPayment");
            DropColumn("EXCEED.PurchasePayment", "ServiceCharge");
            DropColumn("EXCEED.PropertyCount", "BusinessUnitId");
            DropColumn("EXCEED.ProcurementPlan", "BusinessUnitId");
            DropColumn("EXCEED.ItemTransferReceive", "BusinessUnitId");
            DropColumn("EXCEED.ItemTransfer", "BusinessUnitId");
            DropColumn("EXCEED.Issue", "BusinessUnitId");
            DropColumn("EXCEED.Guarantee", "BusinessUnitId");
            DropColumn("EXCEED.Guarantee", "BranchId");
            DropColumn("EXCEED.GoodSellingAndLending", "BusinessUnitId");
            DropColumn("EXCEED.GoodSellingAndLending", "BranchId");
            DropColumn("EXCEED.GoodReceive", "BusinessUnitId");
            DropColumn("EXCEED.PaymentRequest", "AuthorizedTime");
            DropColumn("EXCEED.PaymentRequest", "AuthorizedBy");
            DropColumn("EXCEED.PaymentRequest", "IsAuthorized");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "FixedAssetNameID");
            DropColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetNameID");
            DropColumn("EXCEED.FixedAssetCount", "BusinessUnitId");
            DropColumn("EXCEED.FixedAssetCountItem", "StoreCode");
            DropColumn("EXCEED.FixedAssetCountItem", "EmployeeId");
            DropColumn("EXCEED.FixedAssetCountItem", "AssetStatus");
            DropColumn("EXCEED.FixedAssetCountItem", "Value");
            DropColumn("Inventory_Archive.BinCardLineArchive", "TransactionOrder");
            DropColumn("EXCEED.AssetTransferReceive", "AssetTransferConfirm");
            DropColumn("EXCEED.AssetTransfer", "ConfirmReceive");
            DropColumn("EXCEED.AnnualNeedAssesment", "BusinessUnitId");
            DropColumn("EXCEED.BinCardLine", "TransactionOrder");
            DropColumn("EXCEED.StoreItemAssignment", "Column");
            DropColumn("EXCEED.StoreItemAssignment", "Row");
            DropColumn("EXCEED.StoreItemAssignment", "ShelfNo");
            DropColumn("EXCEED.PaymentMethod", "BusinessUnitId");
            DropColumn("EXCEED.LocationTracking", "DisposalType");
            DropColumn("EXCEED.AssetDisposal", "ExternalCustomer");
            DropColumn("EXCEED.AssetDisposal", "IsExternalCustomer");
            DropColumn("EXCEED.AssetRegistration", "Color");
            DropColumn("EXCEED.AssetRegistration", "SIVDate");
            DropColumn("EXCEED.AssetRegistration", "FixedAssetNameID");
            DropColumn("EXCEED.AssetRegistration", "BusinessUnitId");
            DropTable("EXCEED.StoreRequisitionAttachment");
            DropTable("EXCEED.PaymentRequestAuthorizeValidation");
            CreateIndex("EXCEED.FixedAssetCount", "StoreCode");
            CreateIndex("EXCEED.FixedAssetCount", "BranchId");
            CreateIndex("EXCEED.Disposal", "AssetRegistrationId");
            CreateIndex("EXCEED.BidParticipatingSupplierItem", "LotID");
            CreateIndex("EXCEED.InventoryItem", "UoMName");
            AddForeignKey("EXCEED.InventoryItem", "UoMName", "EXCEED.UoM", "UoMName");
            AddForeignKey("EXCEED.FixedAssetCount", "StoreCode", "EXCEED.InventoryStore", "StoreCode");
            AddForeignKey("EXCEED.FixedAssetCount", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.BidParticipatingSupplierItem", "LotID", "EXCEED.Lot", "LotID", cascadeDelete: true);
        }
    }
}
