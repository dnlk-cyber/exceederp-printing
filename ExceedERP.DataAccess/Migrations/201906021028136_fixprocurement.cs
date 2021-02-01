namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixprocurement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.PurchaseRequisitionDistribution", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.PurchaseRequisitionDistribution", new[] { "PurchaseRequisitionID" });
            CreateTable(
                "EXCEED.PurchaseRequisitionItemSpecification",
                c => new
                    {
                        PurchaseRequisitionItemSpecificationId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionItemID = c.Int(nullable: false),
                        Specification = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionItemSpecificationId)
                .ForeignKey("EXCEED.PurchaseRequisitionItem", t => t.PurchaseRequisitionItemID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionItemID);
            
            AddColumn("EXCEED.LocationTracking", "TransactionType", c => c.String());
            AddColumn("EXCEED.LocationTracking", "TransactionNo", c => c.String());
            AddColumn("EXCEED.Tender", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ContractManagement", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "BranchId", c => c.Int(nullable: false));
            DropColumn("EXCEED.ContractManagement", "Branch");
            DropTable("EXCEED.PurchaseRequisitionDistribution");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.PurchaseRequisitionDistribution",
                c => new
                    {
                        PurchaseRequisitionDistributionID = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionDistributionID);
            
            AddColumn("EXCEED.ContractManagement", "Branch", c => c.String());
            DropForeignKey("EXCEED.PurchaseRequisitionItemSpecification", "PurchaseRequisitionItemID", "EXCEED.PurchaseRequisitionItem");
            DropIndex("EXCEED.PurchaseRequisitionItemSpecification", new[] { "PurchaseRequisitionItemID" });
            DropColumn("EXCEED.PurchaseOrder", "BranchId");
            DropColumn("EXCEED.ProformaPurchase", "BranchId");
            DropColumn("EXCEED.ContractManagement", "BranchId");
            DropColumn("EXCEED.Tender", "BranchId");
            DropColumn("EXCEED.LocationTracking", "TransactionNo");
            DropColumn("EXCEED.LocationTracking", "TransactionType");
            DropTable("EXCEED.PurchaseRequisitionItemSpecification");
            CreateIndex("EXCEED.PurchaseRequisitionDistribution", "PurchaseRequisitionID");
            AddForeignKey("EXCEED.PurchaseRequisitionDistribution", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition", "PurchaseRequisitionID", cascadeDelete: true);
        }
    }
}
