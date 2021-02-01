namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prattachement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PurchaseRequisitionAttachment",
                c => new
                    {
                        PurchaseRequisitionAttachmentId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionAttachmentId)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PurchaseRequisitionAttachment", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.PurchaseRequisitionAttachment", new[] { "PurchaseRequisitionID" });
            DropTable("EXCEED.PurchaseRequisitionAttachment");
        }
    }
}
