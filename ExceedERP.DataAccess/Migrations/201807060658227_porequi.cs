namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class porequi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PoPurchaseRequisition",
                c => new
                    {
                        PoPurchaseRequisitionId = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        StoreRequisitionID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PoPurchaseRequisitionId)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PoPurchaseRequisition", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropIndex("EXCEED.PoPurchaseRequisition", new[] { "PurchaseOrderID" });
            DropTable("EXCEED.PoPurchaseRequisition");
        }
    }
}
