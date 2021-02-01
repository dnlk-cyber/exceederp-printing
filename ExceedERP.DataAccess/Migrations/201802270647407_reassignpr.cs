namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reassignpr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PurchaseRequisitionAssignment",
                c => new
                    {
                        PurchaseRequisitionAssignmentId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        ProcurementOfficer = c.Int(nullable: false),
                        ProcurementMethod = c.Int(nullable: false),
                        PurchaseCategory = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionAssignmentId)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PurchaseRequisitionAssignment", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.PurchaseRequisitionAssignment", new[] { "PurchaseRequisitionID" });
            DropTable("EXCEED.PurchaseRequisitionAssignment");
        }
    }
}
