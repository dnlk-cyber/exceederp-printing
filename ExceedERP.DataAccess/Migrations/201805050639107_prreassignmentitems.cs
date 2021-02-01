namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prreassignmentitems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PurchaseRequisitionReassignmentItem",
                c => new
                    {
                        PurchaseRequisitionReassignmentItemId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionAssignmentId = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        ModelNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemType = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionReassignmentItemId)
                .ForeignKey("EXCEED.PurchaseRequisitionAssignment", t => t.PurchaseRequisitionAssignmentId, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionAssignmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PurchaseRequisitionReassignmentItem", "PurchaseRequisitionAssignmentId", "EXCEED.PurchaseRequisitionAssignment");
            DropIndex("EXCEED.PurchaseRequisitionReassignmentItem", new[] { "PurchaseRequisitionAssignmentId" });
            DropTable("EXCEED.PurchaseRequisitionReassignmentItem");
        }
    }
}
