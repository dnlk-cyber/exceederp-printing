namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropIndex("EXCEED.LotPurchaseRequisition", new[] { "ProformaPurchase_ProformaPurchaseID" });
            AddColumn("EXCEED.LotPurchaseRequisition", "StoreRequisitionID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchaseRequisition", "StoreRequisitionID", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "SroreRequisition", c => c.String());
            DropColumn("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID", c => c.Int());
            DropColumn("EXCEED.PurchaseOrder", "SroreRequisition");
            DropColumn("EXCEED.ProformaPurchaseRequisition", "StoreRequisitionID");
            DropColumn("EXCEED.LotPurchaseRequisition", "StoreRequisitionID");
            CreateIndex("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID");
            AddForeignKey("EXCEED.LotPurchaseRequisition", "ProformaPurchase_ProformaPurchaseID", "EXCEED.ProformaPurchase", "ProformaPurchaseID");
        }
    }
}
