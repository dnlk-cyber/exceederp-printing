namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.GoodReceive", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropIndex("EXCEED.GoodReceive", new[] { "PurchaseOrderID" });
            CreateTable(
                "EXCEED.ForeignPurchaseCost",
                c => new
                    {
                        ForeignPurchaseCostId = c.Int(nullable: false, identity: true),
                        ForeignCost = c.Int(nullable: false),
                        CostValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ForeignPurchaseCostId);
            
            CreateTable(
                "EXCEED.ForeignPurchaseStatus",
                c => new
                    {
                        ForeignPurchaseStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Date = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseStatusId);
            
            CreateTable(
                "EXCEED.ReciveType",
                c => new
                    {
                        ReciveTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Desctiption = c.String(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReciveTypeId);
            
            CreateTable(
                "EXCEED.ItemTransferReciverValidation",
                c => new
                    {
                        ItemTransferReciverValidationID = c.Int(nullable: false, identity: true),
                        ItemTransferID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        Approver = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemTransferReciverValidationID)
                .ForeignKey("EXCEED.ItemTransfer", t => t.ItemTransferID, cascadeDelete: true)
                .Index(t => t.ItemTransferID);
            
            CreateTable(
                "EXCEED.ProjectStores",
                c => new
                    {
                        ProjectStoresId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        StoreCode = c.String(),
                    })
                .PrimaryKey(t => t.ProjectStoresId);
            
            AddColumn("EXCEED.ItemCategory", "Abbreviation", c => c.String());
            AddColumn("EXCEED.StoreItemAssignment", "UOM", c => c.String());
            AddColumn("EXCEED.LotItems", "TaxType", c => c.String());
            AddColumn("EXCEED.LotItems", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchaseRequisitionItem", "UOM", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionPurchaseDepartmentValidation", "Approver", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "Approver", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "UOM", c => c.String());
            AddColumn("EXCEED.StoreRequisitionPropertyValidation", "Approver", c => c.String());
            AddColumn("EXCEED.StoreRequisitionValidation", "Approver", c => c.String());
            AddColumn("EXCEED.GoodReceive", "ReciveTypeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GoodReceive", "ReciveRef", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "UOM", c => c.String());
            AddColumn("EXCEED.GoodReceiveValidation", "Approver", c => c.String());
            AddColumn("EXCEED.GoodReceiveDonationItem", "UOM", c => c.String());
            AddColumn("EXCEED.GoodReciveDonationValidation", "Approver", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingItem", "UOM", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingValidation", "Approver", c => c.String());
            AddColumn("EXCEED.IssueItem", "UOM", c => c.String());
            AddColumn("EXCEED.IssueValidation", "Approver", c => c.String());
            AddColumn("EXCEED.IssueFuel", "PlateNo", c => c.String());
            AddColumn("EXCEED.IssueFuelItem", "UOM", c => c.String());
            AddColumn("EXCEED.IssueFuelValidation", "Approver", c => c.String());
            AddColumn("EXCEED.ItemTransferItem", "UOM", c => c.String());
            AddColumn("EXCEED.ItemTransferValidation", "Approver", c => c.String());
            AddColumn("EXCEED.ParticipatingSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchaseReturnItems", "UOM", c => c.String());
            AddColumn("EXCEED.StockAdjustmentItems", "UOM", c => c.String());
            AddColumn("EXCEED.StockAdjustmentValidation", "Approver", c => c.String());
            AddColumn("EXCEED.StoreReturnItems", "UOM", c => c.String());
            AddColumn("EXCEED.StoreReturnValidation", "Approver", c => c.String());
            AddColumn("EXCEED.TempGoodReceiveItem", "UOM", c => c.String());
            AddColumn("EXCEED.TempGoodReceiveValidation", "Approver", c => c.String());
            AlterColumn("EXCEED.ItemTransferItem", "PartOrCodeNumber", c => c.String());
            CreateIndex("EXCEED.GoodReceive", "ReciveTypeId");
            AddForeignKey("EXCEED.GoodReceive", "ReciveTypeId", "EXCEED.ReciveType", "ReciveTypeId", cascadeDelete: true);
            DropColumn("EXCEED.GoodReceive", "PurchaseOrderID");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GoodReceive", "PurchaseOrderID", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.ItemTransferReciverValidation", "ItemTransferID", "EXCEED.ItemTransfer");
            DropForeignKey("EXCEED.GoodReceive", "ReciveTypeId", "EXCEED.ReciveType");
            DropIndex("EXCEED.ItemTransferReciverValidation", new[] { "ItemTransferID" });
            DropIndex("EXCEED.GoodReceive", new[] { "ReciveTypeId" });
            AlterColumn("EXCEED.ItemTransferItem", "PartOrCodeNumber", c => c.String(nullable: false));
            DropColumn("EXCEED.TempGoodReceiveValidation", "Approver");
            DropColumn("EXCEED.TempGoodReceiveItem", "UOM");
            DropColumn("EXCEED.StoreReturnValidation", "Approver");
            DropColumn("EXCEED.StoreReturnItems", "UOM");
            DropColumn("EXCEED.StockAdjustmentValidation", "Approver");
            DropColumn("EXCEED.StockAdjustmentItems", "UOM");
            DropColumn("EXCEED.PurchaseReturnItems", "UOM");
            DropColumn("EXCEED.ParticipatingSupplier", "TaxAmount");
            DropColumn("EXCEED.ItemTransferValidation", "Approver");
            DropColumn("EXCEED.ItemTransferItem", "UOM");
            DropColumn("EXCEED.IssueFuelValidation", "Approver");
            DropColumn("EXCEED.IssueFuelItem", "UOM");
            DropColumn("EXCEED.IssueFuel", "PlateNo");
            DropColumn("EXCEED.IssueValidation", "Approver");
            DropColumn("EXCEED.IssueItem", "UOM");
            DropColumn("EXCEED.GoodSellingAndLendingValidation", "Approver");
            DropColumn("EXCEED.GoodSellingAndLendingItem", "UOM");
            DropColumn("EXCEED.GoodReciveDonationValidation", "Approver");
            DropColumn("EXCEED.GoodReceiveDonationItem", "UOM");
            DropColumn("EXCEED.GoodReceiveValidation", "Approver");
            DropColumn("EXCEED.GoodReceiveItem", "UOM");
            DropColumn("EXCEED.GoodReceive", "ReciveRef");
            DropColumn("EXCEED.GoodReceive", "ReciveTypeId");
            DropColumn("EXCEED.StoreRequisitionValidation", "Approver");
            DropColumn("EXCEED.StoreRequisitionPropertyValidation", "Approver");
            DropColumn("EXCEED.StoreRequisitionItem", "UOM");
            DropColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "Approver");
            DropColumn("EXCEED.PurchaseRequisitionPurchaseDepartmentValidation", "Approver");
            DropColumn("EXCEED.PurchaseRequisitionItem", "UOM");
            DropColumn("EXCEED.LotItems", "TaxAmount");
            DropColumn("EXCEED.LotItems", "TaxType");
            DropColumn("EXCEED.StoreItemAssignment", "UOM");
            DropColumn("EXCEED.ItemCategory", "Abbreviation");
            DropTable("EXCEED.ProjectStores");
            DropTable("EXCEED.ItemTransferReciverValidation");
            DropTable("EXCEED.ReciveType");
            DropTable("EXCEED.ForeignPurchaseStatus");
            DropTable("EXCEED.ForeignPurchaseCost");
            CreateIndex("EXCEED.GoodReceive", "PurchaseOrderID");
            AddForeignKey("EXCEED.GoodReceive", "PurchaseOrderID", "EXCEED.PurchaseOrder", "PurchaseOrderID", cascadeDelete: true);
        }
    }
}
