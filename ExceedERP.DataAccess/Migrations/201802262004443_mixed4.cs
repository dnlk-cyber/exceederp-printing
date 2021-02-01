namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ProformaPurchase", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.ProformaPurchase", new[] { "PurchaseRequisitionID" });
            CreateTable(
                "EXCEED.ProformaPurchaseRequisition",
                c => new
                    {
                        ProformaPurchaseRequisitionId = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaPurchaseRequisitionId)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            AddColumn("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID", c => c.Int());
            CreateIndex("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID");
            AddForeignKey("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID", "EXCEED.ProformaPurchase", "ProformaPurchaseID");
            DropColumn("HRM.CommitteeMember", "CommitteeType");
            DropColumn("HRM.CommitteeMember", "Date");
            DropColumn("HRM.CommitteeMember", "DateAmharic");
        }
        
        public override void Down()
        {
            AddColumn("HRM.CommitteeMember", "DateAmharic", c => c.String());
            AddColumn("HRM.CommitteeMember", "Date", c => c.DateTime(nullable: false));
            AddColumn("HRM.CommitteeMember", "CommitteeType", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.ProformaPurchaseRequisition", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropIndex("EXCEED.ProformaPurchaseRequisition", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.LotPurchaseRequisition", new[] { "ProformaPurchase_ProformaPurchaseID" });
            DropColumn("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID");
            DropTable("EXCEED.ProformaPurchaseRequisition");
            CreateIndex("EXCEED.ProformaPurchase", "PurchaseRequisitionID");
            AddForeignKey("EXCEED.ProformaPurchase", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition", "PurchaseRequisitionID", cascadeDelete: true);
        }
    }
}
